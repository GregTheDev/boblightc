using System;
using System.IO.Ports;

namespace boblightc
{
    internal class CSerialPort : IDisposable
    {
        private bool disposed = false;
        private SerialPort _serialPort;
        private string m_error;
        private bool m_tostdout;
        private string m_name;

        public CSerialPort()
        {
            _serialPort = new SerialPort();
        }

        internal int Write(byte[] data, int len)
        {
            //fd_set port;

            if (!_serialPort.IsOpen)
            {
                m_error = "port closed";
                return -1;
            }

            int byteswritten = 0;

            try
            {
                _serialPort.Write(data, 0, len);
            }
            catch (Exception serEx)
            {
                m_error = "write() " + serEx.Message;
                return -1;
            }

            #region Original code
            //while (byteswritten < len)
            //{
            //    FD_ZERO(&port);
            //    FD_SET(m_fd, &port);
            //    int returnv = select(m_fd + 1, NULL, &port, NULL, NULL);
            //    if (returnv == -1)
            //    {
            //        m_error = "select() " + GetErrno();
            //        return -1;
            //    }

            //    returnv = write(m_fd, data + byteswritten, len - byteswritten);

            //    if (returnv == -1)
            //    {
            //        m_error = "write() " + GetErrno();
            //        return -1;
            //    }
            //    byteswritten += len;
            //}
            #endregion

            //print what's written to stdout for debugging
            if (m_tostdout)
            {

                Util.Debug($"{m_name} write: ");
                for (int i = 0; i < byteswritten; i++)
                    Util.Debug(String.Format(" %02x", data[i]));

                Util.Debug("\n");
            }

            return byteswritten;
        }

        internal string GetError()
        {
            return m_name + ": " + m_error;
        }

        internal bool Open(string name, int baudrate, int databits = 8, StopBits stopbits = StopBits.One, Parity parity = Parity.None)
        {
            m_name = name;
            m_error = null;

            if (databits < 5 || databits > 8)
            {
                m_error = "Databits has to be between 5 and 8";
                return false;
            }

            if (stopbits != StopBits.One && stopbits != StopBits.Two)
            {
                m_error = "Stopbits has to be 1 or 2";
                return false;
            }

            if (parity != Parity.None && parity != Parity.Even && parity != Parity.Odd)
            {
                m_error = "Parity has to be none, even or odd";
                return false;
            }

            SetBaudRate(baudrate);
            SetPortOptions(databits, stopbits, parity);

            _serialPort.PortName = m_name;
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                m_error = $"Open() {ex.Message}";
                return false;
            }

            //m_fd = open(name.c_str(), O_RDWR | O_NOCTTY);

            //if (m_fd == -1)
            //{
            //    m_error = "open() " + GetErrno();
            //    return false;
            //}

            ////make sure port is blocking
            //int flags = fcntl(m_fd, F_GETFL, 0);
            //fcntl(m_fd, F_SETFL, flags & (~O_NONBLOCK));

            ////set port attributes, don't bail if they fail because the port might still be usable
            //if (tcgetattr(m_fd, &m_options) == 0)
            //{
            //    SetBaudRate(baudrate);
            //    SetPortOptions(databits, stopbits, parity);
            //}
            //else
            //{
            //    m_error = "tcgetattr() " + GetErrno();
            //}

            ////make port non blocking
            //flags = fcntl(m_fd, F_GETFL, 0);
            //fcntl(m_fd, F_SETFL, flags | O_NONBLOCK);

            return true;
        }

        private bool SetPortOptions(int databits, StopBits stopbits, Parity parity)
        {
            try
            {
                //TODO: very different from original code...
                _serialPort.DataBits = databits;
                _serialPort.StopBits = stopbits;
                _serialPort.Parity = parity;
                _serialPort.RtsEnable = true; //TODO: maybe not???
            }
            catch (Exception ex)
            {
                m_error = $"SetPortOptions() {ex.Message}";
                return false;
            }

            return true;
        }

        private bool SetBaudRate(int baudrate)
        {
            try
            {
                _serialPort.BaudRate = baudrate;
            }
            catch (Exception ex)
            {
                m_error = $"SetBaudRate() {ex.Message}";
                return false;
            }

            return true;
        }

        internal bool HasError()
        {
            return !string.IsNullOrEmpty(m_error);
        }

        public void PrintToStdOut(bool tostdout)
        {
            m_tostdout = tostdout;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {

                // Free any other managed objects here.
                //
                _serialPort.Close();
                _serialPort.Dispose();
            }

            disposed = true;
        }
    }
}
