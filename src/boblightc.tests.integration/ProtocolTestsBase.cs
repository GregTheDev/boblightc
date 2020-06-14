using boblightc.Device;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace boblightc.tests.integration
{
    public abstract class ProtocolTestsBase
    {
        protected readonly string _configFileFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SampleConfigFiles");
        protected readonly string _resultsFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SampleResults");
        protected const int LINE_FEED = 10;

        protected CConfig _config;
        protected List<CDevice> _devices;
        protected List<CLight> _lights;
        protected CClientsHandler _clientsHandler;
        protected IPAddress _ipAddress;
        protected int _port;
        protected Socket _socket;
        protected Thread _serverThread;
        protected ManualResetEvent _stopEvent;

        [OneTimeSetUp]
        public void Setup()
        {
            _config = new CConfig();
            _devices = new List<CDevice>();
            _lights = new List<CLight>();
            _clientsHandler = new CClientsHandler(_lights);
            _serverThread = new Thread(new ThreadStart(ServerThreadProc));
            _stopEvent = new ManualResetEvent(false);

            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.ReceiveTimeout = 5000;

            Startup("boblight.conf");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _stopEvent.Set();
            _serverThread.Join();

            if (_socket.Connected)
            {
                _socket.Close();
                _socket.Dispose();
            }

            _lights = null;
            _devices = null;
            _config = null;
            _socket = null;
        }

        protected void Startup(string configFileName)
        {
            bool result;

            result = _config.LoadConfigFromFile(Path.Combine(_configFileFolderPath, configFileName));
            Assert.IsTrue(result);

            result = _config.CheckConfig();
            Assert.IsTrue(result);

            result = _config.BuildConfig(_clientsHandler, _clientsHandler, _devices, _lights);
            Assert.IsTrue(result);

            _clientsHandler.Process(); // First time just opens the server socket
            _serverThread.Start(); // now run the server

            _socket.Connect(_clientsHandler.m_address, _clientsHandler.m_port);
        }

        protected void ServerThreadProc()
        {
            while (!_stopEvent.WaitOne(0))
            {
                _clientsHandler.Process();
            }
        }

        protected void Send(string commandName)
        {
            _socket.Send(Encoding.ASCII.GetBytes($"{commandName}\n"));
        }

        protected string SendAndReceive(string commandName, out byte[] rawData, out int rawDataSize)
        {
            _socket.Send(Encoding.ASCII.GetBytes($"{commandName}\n"));

            byte[] buffer = new byte[1024];
            int bytesReceived = 0;
            StringBuilder response = new StringBuilder();

            do
            {
                bytesReceived = _socket.Receive(buffer);
                response.Append(Encoding.ASCII.GetString(buffer, 0, bytesReceived));

            }
            while (bytesReceived == buffer.Length);

            rawData = buffer;
            rawDataSize = bytesReceived;

            return response.ToString();
        }
    }
}
