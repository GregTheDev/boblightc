using System;
using System.Collections.Generic;

namespace boblightc
{
    internal class CClient
    {
        public CTcpClientSocket m_socket;       //tcp socket for the client
        public CMessageQueue m_messagequeue;
        private List<CLight> m_lights;
        Dictionary<string, int> m_lightnrs;

        public CClient()
        {
            m_socket = new CTcpClientSocket();
            m_lightnrs = new Dictionary<string, int>();
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