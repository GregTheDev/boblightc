using System;

namespace boblightc
{
    internal class CClient
    {
        public CTcpClientSocket m_socket;       //tcp socket for the client
        public CMessageQueue m_messagequeue;

        public CClient()
        {
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}