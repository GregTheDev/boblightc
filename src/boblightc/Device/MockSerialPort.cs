using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace boblightc.Device
{
    public class MockSerialPort : ISerialPort
    {
        private static MockSerialPort _instance;

        public List<byte[]> Writes { get; internal set; }

        public static MockSerialPort Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MockSerialPort();

                return _instance;
            }
        }

        private MockSerialPort() { }

        public bool Open(string name, int baudrate, int databits = 8, StopBits stopbits = StopBits.One, Parity parity = Parity.None)
        {
            Writes = new List<byte[]>();

            return true;
        }

        public int Write(byte[] data, int len)
        {
            Writes.Add((byte[])data.Clone());

            return len;
        }

        public void Dispose()
        {
        }

        public string GetError()
        {
            return "";
        }

        public bool HasError()
        {
            return false;
        }

        public void PrintToStdOut(bool tostdout)
        {
        }

    }
}
