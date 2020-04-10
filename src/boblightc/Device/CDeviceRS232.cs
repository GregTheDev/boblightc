using System;
using System.Collections.Generic;

namespace boblightc.Device
{
    public class CDeviceRS232 : CDevice
    {
        private byte[] m_buff;
        //private List<byte> m_prefix;
        //private List<byte> m_postfix;
        //private int m_max;
        private int m_buffsize;
        private CSignalTimer m_timer;
        private int m_bytes;
        private ISerialPort m_serialport;

        public CDeviceRS232(IChannelDataProvider clients)
            : base(clients)
        {
            m_type = -1;
            m_buff = null;
            m_max = 255;
            m_buffsize = 0;

            m_timer = new CSignalTimer(false); //TODO: m_stop???
        }

        void SetType(int type)
        {
            m_type = type;
            if (type == ATMO) //atmo devices have two bytes for startchannel and one byte for number of channels
            {
                //it doesn't say anywhere if the startchannel is big endian or little endian, so I'm just starting at 0
                m_prefix.Add(0xFF);
                m_prefix.Add(0);
                m_prefix.Add(0);
                m_prefix.Add((byte)m_channels.Count);
            }
            else if (type == KARATE)
            {
                m_prefix.Add(0xAA);
                m_prefix.Add(0x12);
                m_prefix.Add(0);
                m_prefix.Add((byte)m_channels.Count);
            }
            else if (type == SEDU)
            {
                m_prefix.Add(0xA5);
                m_prefix.Add(0x5A);
                m_postfix.Add(0xA1);
            }
        }

        internal override void Sync()
        {
            if (m_allowsync)
                m_timer.Signal();
        }

        protected override bool SetupDevice()
        {
            m_timer.SetInterval(base.Interval);

            if (!OpenSerialPort())
                return false;

            if (m_delayafteropen > 0)
                m_stop.WaitOne(m_delayafteropen / 1000);

            //bytes per channel
            m_bytes = 1;
            while (Math.Round(Math.Pow(256, m_bytes)) <= m_max)
                m_bytes++;

            //allocate a buffer, that can hold the prefix,the number of bytes per channel and the postfix
            m_buffsize = m_prefix.Count + m_channels.Count * m_bytes + m_postfix.Count;
            m_buff = new byte[m_buffsize];

            //copy in the prefix
            if (m_prefix.Count > 0)
                Array.Copy(m_prefix.ToArray(), m_buff, m_prefix.Count);

            //copy in the postfix
            if (m_postfix.Count > 0)
                Array.Copy(m_postfix.ToArray(), 0, m_buff, m_prefix.Count + m_channels.Count * m_bytes, m_postfix.Count);
                //memcpy(m_buff + m_prefix.Count + m_channels.Count * m_bytes, &m_postfix[0], m_postfix.Count);

            //set channel bytes to 0, write it twice to make sure the controller is in sync
            //memset(m_buff + m_prefix.Count, 0, m_channels.Count * m_bytes);
            for (int i = 0; i < 2; i++)
            {
                if (m_serialport.Write(m_buff, m_buffsize) == -1)
                {
                    Util.LogError($"{Name}: {m_serialport.GetError()}");
                    return false;
                }
            }

            return true;
        }

        protected override bool WriteOutput()
        {
            //get the channel values from the clientshandler
            long now = Util.GetTimeUs();
            m_clients.FillChannels(m_channels, now, this);

            //put the values in the buffer, big endian
            for (int i = 0; i < m_channels.Count; i++)
            {
                long output = (long) Math.Round((double)m_channels[i].GetValue(now) * m_max);
                output = Math.Clamp(output, 0, m_max);

                for (int j = 0; j < m_bytes; j++)
                    m_buff[m_prefix.Count + i * m_bytes + j] = (byte) ((output >> ((m_bytes - j - 1) * 8)) & 0xFF);
            }

            //calculate checksum
            if (m_type == KARATE)
            {
                m_buff[2] = (byte) (m_buff[0] ^ m_buff[1]);
                for (int i = 3; i < m_buffsize; i++)
                    m_buff[2] ^= m_buff[i];
            }

            //write the channel values out the serial port
            if (m_serialport.Write(m_buff, m_buffsize) == -1)
            {
                Util.LogError($"{Name}: {m_serialport.GetError()}");
                return false;
            }

            m_timer.Wait();

            return true;
        }

        private bool OpenSerialPort()
        {
            if (Output == "SerialPortMock")
                m_serialport = MockSerialPort.Instance;
            else
                m_serialport = (ISerialPort) new CSerialPort();

            bool opened = m_serialport.Open(Output, Rate);

            if (m_serialport.HasError())
            {
                Util.LogError($"{Name}: {m_serialport.GetError()}");
                if (opened)
                    Util.Log($"{Name}: {Output} had a non fatal error, it might still work, continuing");
            }

            m_serialport.PrintToStdOut(m_debug); //print serial data to stdout when debug mode is on

            return opened;
        }

        protected override void CloseDevice()
        {
            //if opened, set all channels to 0 before closing
            if (m_buff != null)
            {
                Array.Clear(m_buff, m_prefix.Count, m_channels.Count * m_bytes);
                //memset(m_buff + m_prefix.size(), 0, m_channels.size() * m_bytes);
                m_serialport.Write(m_buff, m_buffsize);

                m_buff = null;
                m_buffsize = 0;
            }

            m_serialport.Dispose();
        }
    }
}
