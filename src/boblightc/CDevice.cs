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
        private bool m_allowsync;
        private bool m_debug;
        private int m_delayafteropen;
        private int m_threadpriority;
        private bool m_setpriority;
        private int m_type;
        protected string m_output;
        protected string m_name;

        public CDevice()
            : base()
        {
            m_channels = new List<CChannel>();

            m_allowsync = true;
            m_debug = false;
            m_delayafteropen = 0;
            m_threadpriority = -1;
            m_setpriority = false;
        }

        internal void SetChannel(CChannel channel, int channelnr)
        {
            m_channels[channelnr] = channel;
        }

        internal abstract void Sync();

        public override void Process()
        {
            if (string.IsNullOrEmpty(m_output))
                Util.Log($"{m_name}: starting");
            else
                Util.Log($"{m_name}: starting with output \"{m_output}\"");

            if (m_setpriority)
            {
                //TODO: Going to have to test this... unix supports 1 (low) to 99 (high). .Net supports fixed values from 0 to 4.
                m_thread.Priority = (ThreadPriority) m_threadpriority;
                Util.Log($"{m_name}: successfully set thread priority to {m_threadpriority}");
                //sched_param param = { };
                //param.sched_priority = m_threadpriority;
                //int returnv = pthread_setschedparam(m_thread, SCHED_FIFO, &param);
                //if (returnv == 0)
                //    Log("%s: successfully set thread priority to %i", m_name.c_str(), m_threadpriority);
                //else
                //    LogError("%s: error setting thread priority to %i: %s", m_name.c_str(), m_threadpriority, GetErrno(returnv).c_str());
            }

            long setuptime = 0;

            while (!m_stop.WaitOne(0))
            {
                //keep trying to set up the device every 10 seconds
                while (!m_stop.WaitOne(0))
                {
                    Util.Log($"{m_name}: setting up");

                    setuptime = Util.GetTimeUs();
                    if (!SetupDevice())
                    {
                        CloseDevice();
                        Util.LogError($"{m_name}: setting up failed, retrying in 10 seconds");
                        m_stop.WaitOne(10 * 1000);
                        //USleep(10000000LL, &m_stop);
                    }
                    else
                    {
                        Util.Log($"{m_name}: setup succeeded");
                        break;
                    }
                }

                //keep calling writeoutput until we're asked to stop or writeoutput fails
                while (!m_stop.WaitOne(0))
                {
                    if (!WriteOutput())
                    {
                        //make sure to wait at least one second before trying to set up again
                        long sleepTimeInMicroSeconds = Math.Max(1000000L - (Util.GetTimeUs() - setuptime), 0);
                        Thread.Sleep((int) (sleepTimeInMicroSeconds / 1000));
                        break;
                    }
                }

                CloseDevice();

                Util.Log($"{m_name}: closed");
            }

            Util.Log($"{m_name}: stopped");
        }

        protected virtual bool WriteOutput()
        {
            return false;
        }

        protected virtual void CloseDevice()
        {
            return;
        }

        protected bool SetupDevice()
        {
            return false;
        }
    }
}