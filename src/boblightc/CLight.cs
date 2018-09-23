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

        public int NrColors { get; internal set; }
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
    }
}