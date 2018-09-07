using System;
using System.Collections.Generic;

namespace boblightc
{
    internal class CClientsHandler
    {
        private List<CLight> lights;

        public CClientsHandler(List<CLight> lights)
        {
            this.lights = lights;
        }

        internal void SetInterface(string interfaceAddress, int port)
        {
            this.InterfaceAddress = interfaceAddress;
            this.Port = port;
        }

        public string InterfaceAddress { get; private set; }
        public int Port { get; private set; }

        internal void Process()
        {
            throw new NotImplementedException();
        }
    }
}