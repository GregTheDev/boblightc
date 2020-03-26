using System;
using System.Text;

namespace boblightc
{
    internal class CTcpData
    {
        private byte[] m_data;

        public CTcpData()
        {
            Clear();
        }

        internal byte[] GetData()
        {
            return m_data;
        }

        internal int GetSize()
        {
            return m_data.Length;
        }

        internal void SetData(string data, bool append = false)
        {
            CopyData(Encoding.ASCII.GetBytes(data), data.Length, append);

        }

        internal void SetData(byte[] data, int size, bool append = false)
        {
            CopyData(data, size, append);
        }

        private void CopyData(byte[] data, int size, bool append)
        {
            if (append)
            {
                int start = m_data.Length;// - 1;
                Array.Resize(ref m_data, m_data.Length + size);
                Array.Copy(data, 0, m_data, start, size);
            }
            else
            {
                m_data = new byte[size]; //.resize(size + 1);
                Array.Copy(data, m_data, size);
            }
        }

        internal void Clear()
        {
            m_data = new byte[0];
        }
    }
}
