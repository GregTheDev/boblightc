using System;

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
    }
}