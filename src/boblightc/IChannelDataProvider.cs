using boblightc.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc
{
    public interface IChannelDataProvider
    {
        void FillChannels(List<CChannel> channels, long time, CDevice device);
    }
}
