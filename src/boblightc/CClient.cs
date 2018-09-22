using System;
using System.Collections.Generic;

namespace boblightc
{
    internal class CClient : IDisposable
    {
        public CTcpClientSocket m_socket;       //tcp socket for the client
        public CMessageQueue m_messagequeue;

        internal long m_connecttime;
        internal List<CLight> m_lights;
        private Dictionary<string, int> m_lightnrs;
        internal object m_priority;

        public CClient()
        {
            m_socket = new CTcpClientSocket();
            m_lightnrs = new Dictionary<string, int>();
            m_messagequeue = new CMessageQueue();
        }

        internal void InitLights(List<CLight> lights)
        {
            m_lights = lights;

            //generate a tree for fast lightname->lightnr conversion
            for (int i = 0; i < m_lights.Count; i++)
                m_lightnrs[m_lights[i].Name] = i;
        }

        public void Dispose()
        {
            m_socket.Dispose();
        }

        internal void SetPriority(int priority)
        {
            m_priority = Math.Clamp(priority, 0, 255);
        }

        internal int LightNameToInt(string lightname)
        {
            if (!m_lightnrs.ContainsKey(lightname))
                return -1;
            else
                return m_lightnrs[lightname];
        }
    }
}