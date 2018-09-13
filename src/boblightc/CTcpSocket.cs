using System;
using System.Collections.Generic;
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

        public bool IsOpen { get { return (m_sock != null); } }

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

        protected bool WaitForSocket(bool write, string timeoutstr)
        {
            int timeout = -1;

            //set the timeout
            if (m_usectimeout > 0)
            {
                timeout = m_usectimeout / 1000;
                //timeout.tv_sec = m_usectimeout / 1000000;
                //timeout.tv_usec = m_usectimeout % 1000000;
            }

            List<Socket> rwsock = new List<Socket>();
            rwsock.Add(m_sock);

            try
            {
                if (write)
                    Socket.Select(null, rwsock, null, timeout);
                else
                    Socket.Select(rwsock, null, null, timeout);
            }
            catch (SocketException sockEx)
            {
                m_error = "select() " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                return false;
            }

            //check if the socket had any errors, connection refused is a common one
            try
            {
                object sockstate = m_sock.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);

                if ((int) sockstate != 0)
                {
                    m_error = "SO_ERROR " + m_address + ":" + m_port + " " + sockstate;//  GetErrno(sockstate);
                    return false;
                }
            }
            catch (SocketException sockEx)
            {
                m_error = "getsockopt() " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                return false;
            }

            return true;
        }


        protected bool SetSockOptions()
        {
            //set tcp keepalive
            SetKeepalive();

            //disable nagle algorithm
            int flag = 1;

            try
            {
                m_sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, flag);
            }
            catch (SocketException sockEx)
            {
                m_error = "TCP_NODELAY " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                return false;
            }

            return true;
        }

        private void SetKeepalive()
        {
            int flag = 1;

            m_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, flag);

            //TODO: Keep alive options: https://github.com/dotnet/corefx/issues/25040

            /*
              //two keepalive probes
              flag = 2;
              if (setsockopt(m_sock, IPPROTO_TCP, TCP_KEEPCNT, &flag, sizeof(flag)) == -1)
              {
                m_error = "TCP_KEEPCNT " + GetErrno();
                return FAIL;
              }

              //20 seconds before we start using keepalive
              flag = 20;
              if (setsockopt(m_sock, IPPROTO_TCP, TCP_KEEPIDLE, &flag, sizeof(flag)) == -1)
              {
                m_error = "TCP_KEEPIDLE " + GetErrno();
                return FAIL;
              }

              //20 seconds timeout of each keepalive packet
              flag = 20;
              if (setsockopt(m_sock, IPPROTO_TCP, TCP_KEEPINTVL, &flag, sizeof(flag)) == -1)
              {
                m_error = "TCP_KEEPINTVL " + GetErrno();
                return FAIL;
              }
             */
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