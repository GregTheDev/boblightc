using System;

namespace boblightc
{
    internal class CDeviceRS232 : CDevice
    {
        private CClientsHandler clients;

        public CDeviceRS232(CClientsHandler clients)
            : base()
        {
            this.clients = clients;
        }

        internal override void Sync()
        {
            //if (m_allowsync)
            //    m_timer.Signal();

            throw new System.NotImplementedException();
        }
    }
}