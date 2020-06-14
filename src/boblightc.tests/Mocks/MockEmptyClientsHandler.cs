using boblightc.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc.tests.Mocks
{
    internal class MockEmptyClientsHandler : IChannelDataProvider
    {
        public Dictionary<int, float> ChannelSampleValues { get; set; }

        public MockEmptyClientsHandler(List<CLight> lights, Dictionary<int, float> channelSampleValues)
        {
            ChannelSampleValues = channelSampleValues;
        }

        public void FillChannels(IReadOnlyList<CChannel> channels, long time, CDevice device)
        {
            for (int i = 0; i < channels.Count; i++)
            {
                channels[i].SetValue(ChannelSampleValues[i + 1]);
            }
        }

        public void SetInterface(string interfaceAddress, int port)
        {
            
        }
    }
}
