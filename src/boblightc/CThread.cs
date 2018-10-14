using System;
using System.Threading;

namespace boblightc
{
    internal abstract class CThread : IDisposable
    {
        bool disposed = false;

        protected Thread m_thread;
        //protected volatile bool m_stop;
        protected ManualResetEvent m_stop;
        protected volatile bool m_running;

        public virtual void Process() { }

        protected CThread()
        {
            m_thread = null;
            m_running = false;

            m_stop = new ManualResetEvent(false);
        }

        internal void StartThread()
        {
            m_running = true;
            m_thread = new Thread(new ThreadStart(ThreadFunction));
            m_thread.Start();
        }

        private void ThreadFunction()
        {
            Process();
            m_running = false;
        }

        public void StopThread()
        {
            AsyncStopThread();
            JoinThread();
        }

        public void AsyncStopThread()
        {
            m_stop.Set();
        }

        private void JoinThread()
        {
            if (m_thread != null)
            {
                m_thread.Join();
                m_thread = null;
            }
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                StopThread();
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
    }
}