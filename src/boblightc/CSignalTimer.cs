using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace boblightc
{
    class CSignalTimer : CTimer
    {
        private bool m_signaled;
        //private CCondition m_condition;
        private AutoResetEvent m_condition;

        internal CSignalTimer(bool stop)
            : base(stop)
        {
            m_signaled = false;
            m_condition = new AutoResetEvent(m_signaled);
        }

        public void Wait()
        {
            lock (m_condition)
            {
                //keep looping until we have a timestamp that's not too old
                long now = Util.GetTimeUs();
                long sleeptime;
                do
                {
                    m_time += m_interval;
                    sleeptime = m_time - now;
                }
                while (sleeptime <= m_interval * -2L);

                if (sleeptime > m_interval * 2L) //failsafe, m_time must be bork if we get here
                {
                    sleeptime = m_interval * 2L;
                    Reset();
                }

                //wait for the timeout, or for the condition variable to be signaled
                while (!m_signaled && sleeptime > 0L && m_timerstop == false)
                {
                    System.Diagnostics.Debug.WriteLine($"Sleep: {sleeptime/1000}");

                    //Greg: We need sleeptime in milliseconds, not micro senconds for windows/.net
                    m_condition.WaitOne((int)Math.Min(sleeptime/1000, 1000000/1000));
                    now = Util.GetTimeUs();
                    sleeptime = m_time - now;
                }

                //if we get signaled, reset the timestamp, this allows us to be signaled faster than the interval
                if (m_signaled)
                {
                    Reset();
                    m_signaled = false;
                }
            }

        }

        public void Signal()
        {
            lock (m_condition)
            {
                m_signaled = true;
                m_condition.Set();
            }
        }
    }
}
