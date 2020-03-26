using System;

namespace boblightc
{
    internal class CChannel
    {
        private float m_speed;
        private float m_fallback;
        private long m_lastupdate;
        private float m_wantedvalue;
        private float m_currentvalue;
        private float m_gamma;
        private float m_adjust;
        private float m_blacklevel;
        private float m_singlechange;
        private int m_color;
        private int m_light;

        public int Color { get; internal set; }
        public int Light { get; internal set; }
        public bool m_isused { get; internal set; }

        public CChannel()
        {
            m_color = -1;
            m_light = -1;

            m_isused = false;

            m_speed = 100.0f;
            m_wantedvalue = 0.0f;
            m_currentvalue = 0.0f;
            m_fallback = 0.0f;
            m_lastupdate = -1;
            m_singlechange = 0.0f;

            m_gamma = 1.0f;
            m_adjust = 1.0f;
            m_blacklevel = 0.0f;
        }

        internal float GetValue(long time)
        {
            //we need two calls for the speed
            if (m_lastupdate == -1)
            {
                m_lastupdate = time;
                return m_currentvalue;
            }

            if (m_speed == 100.0) //speed of 100.0 means max
            {
                m_currentvalue = m_wantedvalue;
            }
            else
            {
                float diff = m_wantedvalue - m_currentvalue; //difference between where we want to be, and where we are
                float timediff = time - m_lastupdate; //difference in time in microseconds between now and the last update
                float speed =(float) Math.Pow(2.0, (m_speed - 100.0) / 15.0); //exponential speed, makes the value a lot saner
                                                                  //the value is halved for every 15 counts that speed lowers by
                float speedadjust = 1.0f - (float)Math.Pow(1.0 - speed, timediff / 20000.0); //speed adjustment value, corrected for time

                //move the current value closer to the wanted value
                m_currentvalue += diff * speedadjust;
            }

            if (m_singlechange > 0.0)
            {
                float diff = m_wantedvalue - m_currentvalue;
                m_currentvalue += diff * m_singlechange;
            }

            m_currentvalue = (float) Math.Clamp(m_currentvalue, 0.0, 1.0);

            m_singlechange = 0.0f;
            m_lastupdate = time;

            float outputvalue = m_currentvalue;
            //gamma correction
            if (m_gamma != 1.0)
                outputvalue = (float) Math.Pow(outputvalue, m_gamma);
            //adjust correction
            if (m_adjust != 1.0)
                outputvalue *= m_adjust;
            //blacklevel correction
            if (m_blacklevel != 1.0)
                outputvalue = (float) (outputvalue * (1.0 - m_blacklevel)) + m_blacklevel;

            return outputvalue;
        }

        internal void SetSpeed(float speed)
        {
            m_speed = speed;
        }

        internal void SetValueToFallback()
        {
            m_wantedvalue = m_fallback;
        }

        internal void SetGamma(float gamma)
        {
            m_gamma = gamma;
        }

        internal void SetAdjust(float adjust)
        {
            m_adjust = adjust;
        }

        internal void SetBlacklevel(float blacklevel)
        {
            m_blacklevel = blacklevel;
        }

        internal void SetUsed(bool used)
        {
            m_isused = used;
        }

        internal void SetValue(float value)
        {
            m_wantedvalue = value;
        }

        internal void SetSingleChange(float singlechange)
        {
            //if sync mode is off, then there's no synchronisation between a client writing a singlechange
            //and a device reading a singlechange, so it's possible that a client writes a singlechange
            //and that the client overwrites it again before the device reads it, if we only write singlechange
            //if it's lower than the one set for the channel, it still produces the desired effect
            //m_singlechange is set to 0.0 when the device reads the channel value
            if (singlechange > m_singlechange)
                m_singlechange = singlechange;
        }
    }
}
