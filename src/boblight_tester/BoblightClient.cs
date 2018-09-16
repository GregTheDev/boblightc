using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace boblight_tester
{
    class BoblightClient : IDisposable
    {
        // Flag: Has Dispose already been called?
        bool disposed = false;

        private IPAddress _ipAddress;
        private int _port;
        private Socket _socket;

        public BoblightClient()
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public BoblightClient(string ip, int port)
            : this(IPAddress.Parse(ip), port)
        {

        }

        public BoblightClient(IPAddress ipAddress, int port)
            : this()
        {
            this._ipAddress = ipAddress;
            this._port = port;
        }

        public void Open()
        {
            _socket.Connect(_ipAddress, _port);
        }

        public string Hello()
        {
            return SendAndReceive("hello");
        }

        internal string Ping()
        {
            return SendAndReceive("ping");
        }

        private string SendAndReceive(string commandName)
        {
            _socket.Send(Encoding.ASCII.GetBytes($"{commandName}\n"));

            byte[] buffer = new byte[1024];
            int receivedBytes = _socket.Receive(buffer);

            string response = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

            return response;
        }


        public void Close()
        {
            _socket.Close();
            _socket.Dispose();
        }

        // Public implementation of Dispose pattern callable by consumers.
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
                _socket.Close();
                _socket.Dispose();
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
    }
}
