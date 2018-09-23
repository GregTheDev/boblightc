using System;
using System.Threading;

namespace boblightc
{
    internal abstract class CThread
    {
        protected Thread m_thread;
        //protected volatile bool m_stop;
        protected ManualResetEvent m_stop;
        protected volatile bool m_running;

        public abstract void Process();

        protected CThread()
        {
            m_stop = new ManualResetEvent(false);
        }

        internal void StartThread()
        {
            throw new NotImplementedException();

            //m_stop = false;
            //m_running = true;
            //pthread_create(&m_thread, NULL, ThreadFunction, reinterpret_cast<void*>(this));

        }
    }
}