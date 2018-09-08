using System;
using System.Collections.Generic;

namespace boblightc
{
    internal class CClientsHandler
    {
        private const int SUCCESS = 0; //TODO:xxx
        private const int FAIL = 1;

        private List<CLight> lights;
        private CTcpServerSocket m_socket;

        public string m_address { get; private set; }
        public int m_port { get; private set; }

        public CClientsHandler(List<CLight> lights)
        {
            this.lights = lights;
            this.m_socket = new CTcpServerSocket();
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

            return;

            ////see if there's a socket we can read from
            //List<int> sockets = new List<int>();
            //GetReadableFd(sockets);

            //foreach (int sock in sockets)
            //{
            //    //int sock = *it;
            //    if (sock == m_socket.GetSock()) //we can read from the listening socket
            //    {
            //        CClient client = new CClient();
            //        int returnv = m_socket.Accept(client.m_socket);
            //        if (returnv == SUCCESS)
            //        {
            //            Util.Log($"{client.m_socket.Address}:{client.m_socket.Port} connected");
            //            AddClient(client);
            //        }
            //        else
            //        {
            //            client.Dispose();
            //            client = null;
            //            Util.Log(m_socket.GetError());
            //        }
            //    }
            //    else
            //    {
            //        //get the client the sock fd belongs to
            //        CClient client = GetClientFromSock(sock);
            //        if (client == null) //guess it belongs to nobody
            //            continue;

            //        //try to read data from the client
            //        CTcpData data = null;
            //        int returnv = client.m_socket.Read(data);
            //        if (returnv == FAIL)
            //        { //socket broke probably
            //            Util.Log(client.m_socket.GetError());
            //            RemoveClient(client);
            //            continue;
            //        }

            //        //add data to the messagequeue
            //        client.m_messagequeue.AddData(data.GetData(), data.GetSize());

            //        //check messages from the messaqueue and parse them, if it fails remove the client
            //        if (!HandleMessages(client))
            //            RemoveClient(client);
            //    }
            //}
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
            throw new NotImplementedException();
        }

        private void GetReadableFd(List<int> sockets)
        {
            throw new NotImplementedException();
        }
    }
}