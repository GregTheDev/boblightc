using System;
using System.Net;
using System.Net.Sockets;

namespace boblightc
{
    internal class CTcpServerSocket : CTcpSocket
    {
        public bool Accept(CTcpClientSocket socket)
        {
            if (m_sock == null || !IsOpen)
            {
                m_error = "socket closed";
                return false;
            }

            bool returnv = WaitForSocket(false, "Accept");  //wait for socket to become readable

            if (!returnv) return false;

            try
            {
                Socket acceptedSocket = m_sock.Accept();
                //int sock = accept(m_sock, reinterpret_cast <struct sockaddr*>(&client), &clientlen);

                if (!socket.SetInfo(acceptedSocket))
                {
                    m_error = socket.GetError();
                    return false;
                }
            }
            catch (SocketException sockEx)
            {
                m_error = "select() " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                return false;
            }

            return true;
        }

        internal override bool Open(string address, int port, int usectimeout = -1)
        {
            Close();

            if (String.IsNullOrEmpty(address))
                m_address = "*";
            else
                m_address = address;
  
            m_port = port;
            m_usectimeout = usectimeout;

            IPAddress listenAddress = (m_address == "*") ? IPAddress.Any : IPAddress.Parse(m_address);
            IPEndPoint bindaddr = new IPEndPoint(listenAddress, m_port);

            try
            {
                m_sock = new Socket(bindaddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException sockEx)
            {
                m_error = "socket() " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                return false;
            }

            #region Not ported code
            //TODO: leaving this out?
            // allow socket to be rebound to an existing address
            // https://docs.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-setsockopt
            //int opt = 1;
            //setsockopt(m_sock, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt));

            //TOOD: leaving this out (I just bind to exactly what's in the config file, until this does't work, then I'll change it :) )
            ////bind the socket
            //memset(&bindaddr, 0, sizeof(bindaddr));
            //bindaddr.sin_family = AF_INET;
            //bindaddr.sin_port = htons(m_port);
            //if (address.empty())
            //{
            //    bindaddr.sin_addr.s_addr = htonl(INADDR_ANY);
            //}
            //else
            //{
            //    struct hostent * host = gethostbyname(address.c_str());
            //    if (host == NULL)
            //    {
            //      m_error = "gethostbyname() " + m_address + ":" + ToString(m_port) + " " + GetErrno();
            //      return FAIL;
            //    }
            //    bindaddr.sin_addr.s_addr = * reinterpret_cast<in_addr_t*>(host->h_addr);
            //}
            #endregion

            try
            {
                m_sock.Bind(bindaddr);
            }
            catch (SocketException sockEx)
            {
                m_error = "bind() " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                return false;
            }
            catch (Exception ex)
            {
                m_error = "bind() " + m_address + ":" + m_port + ": " + ex.Message; //TODO: format this better
                return false;

            }

            const int SOMAXCONN = 128; // seems to be the value for unix systems
            //var blah = m_sock.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.MaxConnections);
            try
            {
                m_sock.Listen(SOMAXCONN);
            }
            catch (SocketException sockEx)
            {
                m_error = "listen() " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                return false;
            }
            catch (Exception ex)
            {
                m_error = "listen() " + m_address + ":" + m_port + ": " + ex.Message; //TODO: format this better
                return false;
            }

            SetNonBlock();
  
            return true;
        }

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}
