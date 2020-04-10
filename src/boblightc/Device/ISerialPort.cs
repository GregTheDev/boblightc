using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace boblightc.Device
{
    public interface ISerialPort : IDisposable
    {
        int Write(byte[] data, int len);
        string GetError();
        bool Open(string name, int baudrate, int databits = 8, StopBits stopbits = StopBits.One, Parity parity = Parity.None);
        bool HasError();
        void PrintToStdOut(bool tostdout);


    }
}
