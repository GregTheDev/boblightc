using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace boblightc
{
    class CTimer
    {
        protected long m_interval;
        protected bool m_timerstop;
        protected long m_time;

        protected CTimer(bool stop)
        {
            m_interval = -1;
            m_timerstop = stop;
        }

        public void SetInterval(long usecs)
        {
            m_interval = usecs;
            Reset();
        }

        long GetInterval()
        {
            return m_interval;
        }

        protected void Reset()
        {
            m_time = Util.GetTimeUs();
        }

        void Wait()
        {
            throw new NotImplementedException();

            //long sleeptime;

            ////keep looping until we have a timestamp that's not too old
            //long now = Util.GetTimeUs();
            //do
            //{
            //    m_time += m_interval;
            //    sleeptime = m_time - now;
            //}
            //while (sleeptime <= m_interval * -2L);

            //if (sleeptime > m_interval * 2L) //failsafe, m_time must be bork if we get here
            //{
            //    sleeptime = m_interval * 2L;
            //    Reset();
            //}

            //System.Diagnostics.Debug.Assert(sleeptime >= 1000);

            //m_timerstop.WaitOne((int) (sleeptime / 1000));
        }
    }
}
