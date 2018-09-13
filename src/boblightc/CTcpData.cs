using System;

namespace boblightc
{
    internal class CTcpData
    {
        private byte[] m_data;
        internal byte[] GetData()
        {
            return m_data;
        }

        internal int GetSize()
        {
            return m_data.Length;
        }

        internal void SetData(string v)
        {
            throw new NotImplementedException();
        }
    }
}