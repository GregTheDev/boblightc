using System;
using System.Collections.Generic;
using System.Threading;

namespace boblightc
{
    internal abstract class CDevice : CThread
    {
        public const int NOTHING = 0;
        public const int MOMO = 1;
        public const int ATMO = 2;
        public const int POPEN = 3;
        public const int LTBL = 4;
        public const int SOUND = 5;
        public const int DIODER = 6;
        public const int KARATE = 7;
        public const int IBELIGHT = 8;
        public const int OLA = 9;
        public const int SEDU = 10;
        public const int LPD8806 = 11;
        public const int WS2801 = 12;
        public const int LIGHTPACK = 13;
        public const int AMBIODER = 14;

        public int Type { get; internal set; }
        public string Name { get; internal set; }
        public string Output { get; internal set; }
        public int NrChannels { get { return m_channels.Count; } internal set { m_channels.Clear(); for (int i = 0; i <= value; i++) m_channels.Add(null); }  }
        public int Rate { get; internal set; }
        public int Interval { get; internal set; }
        public List<int> Prefix { get; internal set; }
        public List<int> Postfix { get; internal set; }
        public bool AllowSync { get; internal set; }
        public bool Debug { get; internal set; }
        public Int64 Max { get; internal set; }
        public int DelayAfterOpen { get; internal set; }
        public ThreadPriority ThreadPriority { get; internal set; }

        private List<CChannel> m_channels; //TODO: array might be a better option?

        public CDevice()
        {
            m_channels = new List<CChannel>();
        }

        internal void SetChannel(CChannel channel, int channelnr)
        {
            m_channels[channelnr] = channel;
        }

        internal abstract void Sync();
    }
}