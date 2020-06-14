using boblightc.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc.tests.integration.Mocks
{
    class WalkingChannelProvider : IChannelDataProvider
    {
        private int _nextColor = 0xFF0000;
        private int _nextChannel = 0;

        public int NumberOfLights { get; set; }

        public WalkingChannelProvider()
        {
            _nextChannel = 0;
        }

        public void FillChannels(IReadOnlyList<CChannel> channels, long time, CDevice device)
        {
            for (int j = 0; j < channels.Count; j++)
            {
                if (j == _nextChannel)
                {
                    channels[j].SetValue(_nextColor == 0xFF0000 ?  1f : 0f); // red
                    j++;
                    channels[j].SetValue(_nextColor == 0x00FF00 ? 1f : 0f); // green
                    j++;
                    channels[j].SetValue(_nextColor == 0x0000FF ? 1f : 0f); // blue
                }
                else
                {
                    channels[j].SetValue(0f); // red
                    j++;
                    channels[j].SetValue(0f); // green
                    j++;
                    channels[j].SetValue(0f); // blue
                }
            }

            if (_nextColor == 0xFF0000)
                _nextColor = 0x00FF00;
            else if (_nextColor == 0x00FF00)
                _nextColor = 0x0000FF;
            else
                _nextColor = 0xFF0000;

            _nextChannel += 3;

            if (_nextChannel >= channels.Count)
                _nextChannel = 0;
        }
    }
}
