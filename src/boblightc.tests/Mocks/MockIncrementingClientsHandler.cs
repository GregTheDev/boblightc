using boblightc.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc.tests.Mocks
{
    internal class MockIncrementingClientsHandler : IChannelDataProvider
    {
        private int _testingCounter;

        public MockIncrementingClientsHandler(List<CLight> lights)
        {
            _testingCounter = 1;
        }

        public void FillChannels(List<CChannel> channels, long time, CDevice device)
        {
            for (int i = 0; i < channels.Count; i++)
            {
                if (_testingCounter >= 255)
                    _testingCounter = 1;

                channels[i].SetValue(_testingCounter / 255.0f);

                _testingCounter++;
            }
        }

        public void SetInterface(string interfaceAddress, int port)
        {
            
        }
    }
}
