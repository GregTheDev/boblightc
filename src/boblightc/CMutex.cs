using System;
using System.Threading;

namespace boblightc
{
    //TODO: switch to native locking
    internal class CMutex
    {
        private Mutex _mutex;
        private int m_refcount;
        // Flag: Has Dispose already been called?
        protected bool disposed = false;

        public CMutex()
        {
            _mutex = new Mutex();

            m_refcount = 0;
        }

        public void Unlock()
        {
            m_refcount--;

            System.Diagnostics.Debug.Assert(m_refcount >= 0);

            _mutex.ReleaseMutex();
        }

        public bool TryLock()
        {
            if (_mutex.WaitOne(1))
            {
                m_refcount++;
                System.Diagnostics.Debug.Assert(m_refcount > 0);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Lock()
        {
            _mutex.WaitOne();
            m_refcount++;
            System.Diagnostics.Debug.Assert(m_refcount >0);
            return true;
        }

        //TODO: use IDisposable
        ~CMutex()
        {
            _mutex.Dispose();
        }
    }
}