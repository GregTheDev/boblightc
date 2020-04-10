using boblightc.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc
{
    public interface IServerConfiguration
    {
        void SetInterface(string interfaceAddress, int port);
    }
}
