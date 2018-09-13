using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc
{
    class CLock
    {
        private CMutex m_mutex;
        private bool m_haslock;

        public CLock(CMutex m_mutex)
        {
            this.m_mutex = m_mutex;
            m_haslock = false;

            Enter();
        }

        internal void Enter()
        {
            if (!m_haslock)
            {
                m_mutex.Lock();
                m_haslock = true;
            }
        }

        internal void Leave()
        {
            if (m_haslock)
            {
                m_mutex.Unlock();
                m_haslock = false;
            }
        }

        ~CLock()
        {
            Leave();
        }
    }
}
