using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace boblightc
{
    internal class CClientsHandler
    {
        private const int SUCCESS = 0; //TODO:xxx
        private const int FAIL = 1;

        private List<CLight> m_lights;
        private List<CClient> m_clients;
        private CTcpServerSocket m_socket;

        public string m_address { get; private set; }
        public int m_port { get; private set; }
        private CMutex m_mutex;

        public CClientsHandler(List<CLight> lights)
        {
            this.m_lights = lights;
            this.m_socket = new CTcpServerSocket();
            this.m_clients = new List<CClient>();
            this.m_mutex = new CMutex();
        }

        internal void SetInterface(string interfaceAddress, int port)
        {
            this.m_address = interfaceAddress;
            this.m_port = port;
        }

        internal void Process()
        {
            //open listening socket if it's not already
            if (!m_socket.IsOpen)
            {
                string liseningAddress = String.IsNullOrEmpty(m_address) ? "*" : m_address;
                Util.Log($"opening listening socket on {liseningAddress}:{m_port}");

                if (!m_socket.Open(m_address, m_port, 1000000))
                {
                    Util.LogError($"{m_socket.GetError()}");
                    m_socket.Close();
                }
            }

            //see if there's a socket we can read from
            IList<Socket> sockets = GetReadableFd();

            foreach (Socket sock in sockets)
            {
                if (sock == m_socket.GetSock()) //we can read from the listening socket
                {
                    CClient client = new CClient();
                    bool returnv = m_socket.Accept(client.m_socket);
                    if (returnv)
                    {
                        Util.Log($"{client.m_socket.Address}:{client.m_socket.Port} connected");
                        AddClient(client);
                    }
                    else
                    {
                        client.Dispose();
                        client = null;
                        Util.Log(m_socket.GetError());
                    }
                }
                else
                {
                    throw new NotImplementedException("Boo");
                    ////get the client the sock fd belongs to
                    //CClient client = GetClientFromSock(sock);
                    //if (client == null) //guess it belongs to nobody
                    //    continue;

                    ////try to read data from the client
                    //CTcpData data = null;
                    //int returnv = client.m_socket.Read(data);
                    //if (returnv == FAIL)
                    //{ //socket broke probably
                    //    Util.Log(client.m_socket.GetError());
                    //    RemoveClient(client);
                    //    continue;
                    //}

                    ////add data to the messagequeue
                    //client.m_messagequeue.AddData(data.GetData(), data.GetSize());

                    ////check messages from the messaqueue and parse them, if it fails remove the client
                    //if (!HandleMessages(client))
                    //    RemoveClient(client);
                }
            }
        }

        internal IList<Socket> GetReadableFd()
        {
            const int WAITTIME = 10000000;
            CLock _lock = new CLock(m_mutex);

            //no clients so we just sleep
            if (m_clients.Count == 0 && !m_socket.IsOpen)
            {
                _lock.Leave();
                System.Threading.Thread.Sleep(1 * 1000); //TODO: change to manual reset event
                //USleep(WAITTIME, &g_stop);
                return new List<Socket>();
            }

            //store all the client sockets
            List<Socket> waitsockets = new List<Socket>();
            waitsockets.Add(m_socket.GetSock());
            for (int i = 0; i < m_clients.Count; i++)
                waitsockets.Add(m_clients[i].m_socket.GetSock());

            _lock.Leave();

            //struct timeval tv;
            //tv.tv_sec = WAITTIME / 1000000;
            //tv.tv_usec = (WAITTIME % 1000000);

            try
            {
                Socket.Select(waitsockets, null, null, WAITTIME / 1000);
            }
            catch (SocketException sockEx)
            {
                Util.LogError($"select() " + sockEx.NativeErrorCode + " " + sockEx.SocketErrorCode); //TODO: format this better
                return new List<Socket>();
            }

            return waitsockets;
        }

        internal void Cleanup()
        {
#warning  FINISH THIS!!!!
            //kick off all clients
            Util.Log("disconnecting clients");
            //TODO: need to do this
            //CLock lock (m_mutex) ;
            //while (m_clients.size())
            //{
            //    RemoveClient(m_clients.front());
            //}
            //lock.Leave();

            Util.Log("closing listening socket");
            m_socket.Close();

            Util.Log("clients handler stopped");
        }

        private bool HandleMessages(CClient client)
        {
            throw new NotImplementedException();
        }

        private void RemoveClient(CClient client)
        {
            throw new NotImplementedException();
        }

        private CClient GetClientFromSock(int sock)
        {
            throw new NotImplementedException();
        }

        private void AddClient(CClient client)
        {
            //TODO: google this
            const int FD_SETSIZE = 100;

            CLock _lock = new CLock(m_mutex);

            if (m_clients.Count >= FD_SETSIZE) //maximum number of clients reached
            {
                Util.LogError($"number of clients reached maximum {FD_SETSIZE}");
                CTcpData data = new CTcpData();
                data.SetData("full\n");
                client.m_socket.Write(data);
                client = null;
                return;
            }

            //assign lights and put the pointer in the clients vector
            client.InitLights(m_lights);
            m_clients.Add(client);
        }
    }
}