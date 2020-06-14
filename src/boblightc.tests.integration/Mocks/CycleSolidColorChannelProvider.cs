using boblightc.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc.tests.integration.Mocks
{
    class CycleSolidColorChannelProvider : IChannelDataProvider
    {
        private int _executions;
        private int _nextChange = 100;
        private float _red;
        private float _green;
        private float _blue;

        public CycleSolidColorChannelProvider()
        {
            _red = 1f;
            _green = 0f;
            _blue = 0f;
        }

        public void FillChannels(IReadOnlyList<CChannel> channels, long time, CDevice device)
        {
            for (int i = 0; i < channels.Count; i++)
            {
                channels[i].SetValue(_red);
                i++;
                channels[i].SetValue(_green);
                i++;
                channels[i].SetValue(_blue);
            }

            _executions++;

            if (_executions >= _nextChange)
            {
                _nextChange += 100;

                if (_red == 1f)
                {
                    _red = 0f;
                    _green = 1f;
                    _blue = 0f;
                }
                else if (_green == 1f)
                {
                    _red = 0f;
                    _green = 0f;
                    _blue = 1f;
                }
                else
                {
                    _red = 1f;
                    _green = 0f;
                    _blue = 0f;
                }
            }
        }
    }
}
