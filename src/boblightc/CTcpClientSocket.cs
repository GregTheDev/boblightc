using System;
using System.Net;
using System.Net.Sockets;

namespace boblightc
{
    internal class CTcpClientSocket : CTcpSocket
    {
        internal bool Read(CTcpData data)
        {
            byte[] buff= new byte[1000];

            if (m_sock == null)
            {
                m_error = "socket closed";
                return false;
            }

            bool returnv = WaitForSocket(false, "Read");//wait until the socket has something to read
            if (!returnv)
                return returnv;

            //clear the tcpdata
            data.Clear();

            //loop until the socket has nothing more in its buffer
            while (true)
            {
                try
                {
                    int size = m_sock.Receive(buff, 0, buff.Length, SocketFlags.None, out SocketError errorCode); // recv(m_sock, buff, sizeof(buff), 0);

                    if (errorCode == SocketError.TryAgain && size == -1) //we're done here, no more data, the call to WaitForSocket made sure there was at least some data to read
                    {
                        return true;
                    }
                    else if (size == -1) //socket had an error
                    {
                        //m_error = "recv() " + m_address + ":" + m_port + " " + GetErrno();
                        return false;
                    }
                    else if (size == 0 && data.GetSize() == 0) //socket closed and no data received
                    {
                        m_error = m_address + ":" + m_port + " Connection closed";
                        return false;
                    }
                    else if (size == 0) //socket closed but data received
                    {
                        return true;
                    }

                    data.SetData(buff, size, true); //append the data
                }
                catch (SocketException sockEx)
                {
                    m_error = "receive() " + m_address + ":" + m_port + " " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode; //TODO: format this better
                    return false;
                }
            }
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

        internal bool Write(CTcpData data)
        {
            if (m_sock == null)
            {
                m_error = "socket closed";
                return false;
            }

            int bytestowrite = data.GetSize();
            int byteswritten = 0;

            //loop until we've written all bytes
            while (byteswritten < bytestowrite)
            {
                //wait until socket becomes writeable
                bool returnv = WaitForSocket(true, "Write");

                if (!returnv)
                    return returnv;

                int size = m_sock.Send(data.GetData(), byteswritten, data.GetSize() - byteswritten, SocketFlags.None, out SocketError sockError);

                if (size == -1)
                {
                    m_error = "send() " + m_address + ":" + m_port + " " + sockError;
                    return false;
                }

                byteswritten += size;
            }
            return true;
        }
    }
}
