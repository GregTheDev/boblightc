using System;
using System.Collections.Generic;

namespace boblightc
{
    internal class CClient
    {
        public CTcpClientSocket m_socket;       //tcp socket for the client
        public CMessageQueue m_messagequeue;

        internal long m_connecttime;
        private List<CLight> m_lights;
        private Dictionary<string, int> m_lightnrs;

        public CClient()
        {
            m_socket = new CTcpClientSocket();
            m_lightnrs = new Dictionary<string, int>();
            m_messagequeue = new CMessageQueue();
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }

        internal void InitLights(List<CLight> lights)
        {
            m_lights = lights;

            //generate a tree for fast lightname->lightnr conversion
            for (int i = 0; i < m_lights.Count; i++)
                m_lightnrs[m_lights[i].Name] = i;
        }
    }
}