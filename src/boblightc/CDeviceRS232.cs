namespace boblightc
{
    internal class CDeviceRS232 : CDevice
    {
        private CClientsHandler clients;

        public CDeviceRS232(CClientsHandler clients)
        {
            this.clients = clients;
        }
    }
}