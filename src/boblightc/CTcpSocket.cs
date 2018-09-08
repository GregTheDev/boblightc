using System;
using System.Net.Sockets;

namespace boblightc
{
    internal abstract class CTcpSocket : IDisposable
    {
        protected Socket m_sock;
        protected string m_address;
        protected int m_port;
        protected int m_usectimeout;
        protected string m_error;

        // Flag: Has Dispose already been called?
        protected bool disposed = false;

        public bool IsOpen { get { return (m_sock == null) ? false : m_sock.Connected; } }

        internal string Address { get { return m_address; } }
        internal int Port { get { return m_port; } }


        internal void Close()
        {
            if (m_sock != null && m_sock.Connected)
            {
                //SetNonBlock(false);
                m_sock.Close();
                m_sock = null;
            }
        }

        internal abstract bool Open(string address, int port, int usectimeout = -1);

        internal string GetError()
        {
            return m_error;
        }

        internal Socket GetSock()
        {
            return m_sock;
        }

        protected bool SetNonBlock()
        {
            m_sock.Blocking = false;

            return true;
        }

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

                // Free any other managed objects here.
                Close();
            }

            disposed = true;
        }
    }
}