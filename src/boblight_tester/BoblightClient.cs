using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace boblight_tester
{
    class BoblightClient
    {
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

        public void Close()
        {
            _socket.Close();
            _socket.Dispose();
        }
    }
}
