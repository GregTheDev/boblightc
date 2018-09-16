using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;

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
                    //get the client the sock fd belongs to
                    CClient client = GetClientFromSock(sock);
                    if (client == null) //guess it belongs to nobody
                        continue;

                    //try to read data from the client
                    CTcpData data = new CTcpData();
                    bool returnv = client.m_socket.Read(data);
                    if (!returnv)
                    { //socket broke probably
                        Util.Log(client.m_socket.GetError());
                        RemoveClient(client);
                        continue;
                    }

                    //add data to the messagequeue
                    client.m_messagequeue.AddData(data.GetData(), data.GetSize());

                    //check messages from the messaqueue and parse them, if it fails remove the client
                    if (!HandleMessages(client))
                        RemoveClient(client);
                }
            }
        }

        internal IList<Socket> GetReadableFd()
        {
            const int WAITTIME = 10000000;
            List<Socket> waitsockets = new List<Socket>();

            lock (m_mutex)
            {

                //no clients so we just sleep
                if (m_clients.Count == 0 && !m_socket.IsOpen)
                {
                    System.Threading.Thread.Sleep(1 * 1000); //TODO: change to manual reset event
                                                             //USleep(WAITTIME, &g_stop);
                    return new List<Socket>();
                }

                //store all the client sockets
                waitsockets.Add(m_socket.GetSock());
                for (int i = 0; i < m_clients.Count; i++)
                    waitsockets.Add(m_clients[i].m_socket.GetSock());
            }

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
            //kick off all clients
            Util.Log("disconnecting clients");
            
            lock (m_mutex)
            {
                while (m_clients.Count > 0)
                {
                    RemoveClient(m_clients.First());
                }
            }

            Util.Log("closing listening socket");
            m_socket.Close();

            Util.Log("clients handler stopped");
        }

        private bool HandleMessages(CClient client)
        {
            if (client.m_messagequeue.GetRemainingDataSize() > CMessageQueue.MAXDATA) //client sent too much data
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent too much data");
                return false;
            }

            //loop until there are no more messages
            while (client.m_messagequeue.GetNrMessages() > 0)
            {
                CMessage message = client.m_messagequeue.GetMessage();
                if (!ParseMessage(client, message))
                    return false;
            }
            return true;
        }

        private bool ParseMessage(CClient client, CMessage message)
        {
            CTcpData data = new CTcpData();
            string messagekey;
            //an empty message is invalid
            if (!Util.GetWord(ref message.message, out messagekey))
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
            }

            if (messagekey == "hello")
            {
                Util.Log($"{client.m_socket.Address}:{client.m_socket.Port} said hello");
                data.SetData("hello\n");
                if (client.m_socket.Write(data) != true)
                {
                    Util.Log(client.m_socket.GetError());
                    return false;
                }

                lock (m_mutex)
                {

                    if (client.m_connecttime == -1)
                        client.m_connecttime = message.time;
                }
            }
            else if (messagekey == "ping")
            {
                Util.Log($"{client.m_socket.Address}:{client.m_socket.Port} said ping");

                return SendPing(client);
            }
            else if (messagekey == "get")
            {
                Util.Log($"{client.m_socket.Address}:{client.m_socket.Port} said get");

                return ParseGet(client, message);
            }

            return true;
        }

        private bool ParseGet(CClient client, CMessage message)
        {
            CTcpData data = new CTcpData();
            string messagekey;
            if (!Util.GetWord(ref message.message, out messagekey))
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
            }

            if (messagekey == "version")
            {
                return SendVersion(client);
            }
            else if (messagekey == "lights")
            {
                return SendLights(client);
            }
            else
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
            }
        }

        private bool SendLights(CClient client)
        {
            throw new NotImplementedException();
        }

        private bool SendVersion(CClient client)
        {
            CTcpData data = new CTcpData();

            data.SetData("version " + ProtocolVersion.Version + "\n");

            if (client.m_socket.Write(data) != true)
            {
                Util.Log(client.m_socket.GetError());
                return false;
            }
            return true;
        }

        private bool SendPing(CClient client)
        {
            int lightsused = 0;

            lock (m_mutex)
            {

                //check if any light is used
                for (int i = 0; i < client.m_lights.Count; i++)
                {
                    if (client.m_lights[i].GetNrUsers() > 0)
                    {
                        lightsused = 1;
                        break; //if one light is used we have enough info
                    }
                }
            }

            CTcpData data = new CTcpData();
            data.SetData("ping " + lightsused + "\n");

            if (client.m_socket.Write(data) != true)
            {
                Util.Log(client.m_socket.GetError());
                return false;
            }
            return true;
        }

        private void RemoveClient(CClient client)
        {
            throw new NotImplementedException();
        }

        private CClient GetClientFromSock(Socket sock)
        {
            lock(m_mutex)
            {
                var possibleMatch = m_clients.FirstOrDefault(client => client.m_socket.GetSock() == sock);

                return possibleMatch;
            }
        }

        private void AddClient(CClient client)
        {
            //TODO: google this
            const int FD_SETSIZE = 100;

            lock (m_mutex)
            {
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
}