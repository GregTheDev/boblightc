using boblightc.Device;
using System;
using System.Collections.Generic;

namespace boblightc
{
    internal class CLight
    {
        private float[] m_hscan = new float[2];
        private float[] m_vscan = new float[2];
        private List<CColor> m_colors;
        private long m_time;
        private long m_prevtime;
        private float[] m_rgb = new float[3];
        private float[] m_prevrgb =  new float[3];
        private float m_speed;
        private bool m_use;
        private bool m_interpolation;

        public int NrColors { get { return m_colors.Count; } }
        public string Name { get; internal set; }
        public Dictionary<CDevice, float> m_users;

        public CLight()
        {
            m_colors = new List<CColor>();
            m_users = new Dictionary<CDevice, float>();
            m_time = -1;
            m_prevtime = -1;

            for (int i = 0; i < 3; i++)
            {
                m_rgb[i] = 0.0f;
                m_prevrgb[i] = 0.0f;
            }

            m_speed = 100.0f;
            m_use = true;
            m_interpolation = false;

            m_hscan[0] = 0.0f;
            m_hscan[1] = 100.0f;
            m_vscan[0] = 0.0f;
            m_vscan[1] = 100.0f;
        }

        internal void AddColor(CColor color)
        {
            this.m_colors.Add(color);
        }

        internal void SetHscan(float[] hscan)
        {
            this.m_hscan = hscan;
        }

        internal void SetVscan(float[] vscan)
        {
            this.m_vscan = vscan;
        }

        internal int GetNrUsers()
        {
            return m_users.Count;
        }

        internal float[] GetVscan()
        {
            return m_vscan;
        }

        internal float[] GetHscan()
        {
            return m_hscan;
        }

        internal void SetRgb(float[] rgb, long time)
        {
            for (int i = 0; i < 3; i++)
                rgb[i] = Math.Clamp(rgb[i], 0.0f, 1.0f);

            m_prevrgb = (float[]) m_rgb.Clone();
            m_rgb = (float[]) rgb.Clone();

            m_prevtime = m_time;
            m_time = time;
        }

        internal void SetSpeed(float speed)
        {
            m_speed = Math.Clamp(speed, 0.0f, 100.0f);
        }

        internal void SetInterpolation(bool interpolation)
        {
            m_interpolation = interpolation;
        }

        internal void SetUse(bool use)
        {
            m_use = use;
        }

        internal void SetSingleChange(float singlechange)
        {
            foreach (var key in m_users.Keys)
                m_users[key] = Math.Clamp(singlechange, 0.0f, 1.0f);

            //for (uint i = 0; i < m_users.Count; i++)
            //    m_users.[i].second = Math.Clamp(singlechange, 0.0, 1.0);
        }

        internal CDevice GetUser(int j)
        {
            //GN: Created a new GetAllUsers function
            throw new NotImplementedException();
        }

        internal IEnumerable<CDevice> GetAllUsers()
        {
            return m_users.Keys;
        }

        internal bool GetUse()
        {
            return m_use;
        }

        internal float GetSpeed()
        {
            return m_speed;
        }

        internal float GetColorValue(int colornr, long time)
        {
            if (m_interpolation && m_prevtime == -1) //need two writes for interpolation
                return 0.0f;

            float[] rgb = new float[3];
            if (m_interpolation)
            {
                float multiply = 0.0f;
                if ((float)(m_time - m_prevtime) > 0.0) //don't want to divide by 0
                {
                    multiply = (float)(time - m_time) / (float)(m_time - m_prevtime);
                }
                multiply = (float) Math.Clamp(multiply, 0.0, 1.0);
                for (int i = 0; i < 3; i++)
                {
                    float diff = m_rgb[i] - m_prevrgb[i];
                    rgb[i] = m_prevrgb[i] + (diff * multiply);
                }
            }
            else
            {
                Array.Copy(m_rgb, rgb, rgb.Length);
            }

            if (rgb[0] == 0.0 && rgb[1] == 0.0 && rgb[2] == 0.0)
                return 0.0f;

            float[] maxrgb = { 0.0f, 0.0f, 0.0f };
            for (int i = 0; i < m_colors.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                    maxrgb[j] += m_colors[i].GetRgb()[j];
            }

            float expandvalue = FindMultiplier(rgb, 1.0f);
            for (int i = 0; i < 3; i++)
                rgb[i] *= expandvalue;

            float range = FindMultiplier(rgb, maxrgb);
            for (int i = 0; i < 3; i++)
                rgb[i] *= range;

            float colorvalue = 0.0f;
            for (int i = 0; i <= colornr; i++)
            {
                colorvalue = FindMultiplier(m_colors[i].GetRgb(), rgb);
                colorvalue = (float) Math.Clamp(colorvalue, 0.0, 1.0);

                for (int j = 0; j < 3; j++)
                {
                    rgb[j] -= m_colors[i].GetRgb()[j] * colorvalue;
                }
            }

            return colorvalue / expandvalue;
        }

        private float FindMultiplier(float[] rgb, float[] ceiling)
        {
            float multiplier = float.MaxValue;

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] > 0.0)
                {
                    if (ceiling[i] / rgb[i] < multiplier)
                        multiplier = ceiling[i] / rgb[i];
                }
            }
            return multiplier;
        }

        private float FindMultiplier(float[] rgb, float ceiling)
        {
            float multiplier = float.MaxValue;

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] > 0.0)
                {
                    if (ceiling / rgb[i] < multiplier)
                        multiplier = ceiling / rgb[i];
                }
            }
            return multiplier;
        }

        internal float GetGamma(int colornr)
        {
            return m_colors[colornr].GetGamma();
        }

        internal float GetAdjust(int colornr)
        {
            return m_colors[colornr].GetAdjust();
        }

        internal float GetBlacklevel(int colornr)
        {
            return m_colors[colornr].GetBlacklevel();
        }

        internal float GetSingleChange(CDevice device)
        {
            foreach (CDevice userDevice in m_users.Keys)
            {
                if (userDevice == device)
                    return m_users[userDevice];

            }
            //for (int i = 0; i < m_users.Count; i++)
            //{
            //    if (m_users.Keys[i] == device)
            //        return m_users[i].second;
            //}

            return 0.0f;
        }

        internal void ResetSingleChange(CDevice device)
        {
            foreach (CDevice userDevice in m_users.Keys)
            {
                if (userDevice == device)
                {
                    m_users[userDevice] = 0.0f;
                    return;
                }
            }
            //for (int i = 0; i < m_users.Count; i++)
            //{
            //    if (m_users[i].first == device)
            //    {
            //        m_users[i].second = 0.0;
            //        return;
            //    }
            //}
        }

        internal void AddUser(CDevice device)
        {
            //add CDevice pointer to users if it doesn't exist yet
            foreach (CDevice userDevice in m_users.Keys)
            {
                if (userDevice == device)
                    return;
            }

            //for (unsigned int i = 0; i < m_users.size(); i++)
            //{
            //    if (m_users[i].first == device)
            //        return;
            //}

            m_users.Add(device, 0.0f);
        }

        internal void ClearUser(CDevice device)
        {
            //clear CDevice* from users
            if (m_users.ContainsKey(device))
                m_users.Remove(device);
            //for (vector<pair<CDevice*, float>>::iterator it = m_users.begin(); it != m_users.end(); it++)
            //{
            //    if ((*it).first == device)
            //    {
            //        m_users.erase(it);
            //        return;
            //    }
            //}
        }
    }
}
