using System;
using System.Net;
using System.Net.Sockets;

namespace boblightc
{
    internal class CTcpClientSocket : CTcpSocket
    {
        internal int Read(CTcpData data)
        {
            throw new NotImplementedException();
        }

        internal override bool Open(string address, int port, int usectimeout = -1)
        {
            throw new NotImplementedException();
        }

        internal bool SetInfo(Socket socket)
        {
            IPEndPoint remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;

            m_address = remoteEndPoint.Address.ToString();
            m_port = remoteEndPoint.Port;
            m_sock = socket;

            bool returnv = SetNonBlock();
            if (!returnv)
                return returnv;

            returnv = SetSockOptions();

            return returnv;

        }

        internal void Write(CTcpData data)
        {
            throw new NotImplementedException();
        }
    }
}