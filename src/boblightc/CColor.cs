using System;

namespace boblightc
{
    internal class CColor
    {
        public string Name { get; internal set; }
        public float[] Rgb { get; internal set; }
        public float m_gamma { get; internal set; }
        public float m_adjust { get; internal set; }
        public float m_blacklevel { get; internal set; }

        public CColor()
        {
            Rgb = new float[3];
            m_gamma = 1.0f;
            m_adjust = 1.0f;
            m_blacklevel = 0.0f;
        }

        internal float GetGamma()
        {
            return m_gamma;
        }

        internal float GetAdjust()
        {
            return m_adjust;
        }

        internal float GetBlacklevel()
        {
            return m_blacklevel;
        }

        internal float[] GetRgb()
        {
            return Rgb;
        }
    }
}