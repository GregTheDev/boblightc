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
        private double m_speed;
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

            m_speed = 100.0;
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
    }
}