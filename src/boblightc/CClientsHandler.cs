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

        public void FillChannels(List<CChannel> channels, long time, CDevice device)
        {
            List<CLight> usedlights = new List<CLight>();

            lock (m_mutex)
            {

                //get the oldest client with the highest priority
                for (int i = 0; i < channels.Count; i++)
                {
                    long clienttime = 0x7fffffffffffffff;
                    int priority = 255;
                    int light = channels[i].Light;
                    int color = channels[i].Color;
                    int clientnr = -1;

                    if (light == -1 || color == -1) //unused channel
                        continue;

                    for (int j = 0; j < m_clients.Count; j++)
                    {
                        if (m_clients[j].m_priority == 255 || m_clients[j].m_connecttime == -1 || !m_clients[j].m_lights[light].GetUse())
                            continue; //this client we don't use

                        //this client has a high priority (lower number) than the current one, or has the same and is older
                        if (m_clients[j].m_priority < priority || (priority == m_clients[j].m_priority && m_clients[j].m_connecttime < clienttime))
                        {
                            clientnr = j;
                            clienttime = m_clients[j].m_connecttime;
                            priority = m_clients[j].m_priority;
                        }
                    }

                    if (clientnr == -1) //no client for the light on this channel
                    {
                        channels[i].m_isused = false;
                        channels[i].SetSpeed(m_lights[light].GetSpeed());
                        channels[i].SetValueToFallback();
                        channels[i].SetGamma(1.0f);
                        channels[i].SetAdjust(1.0f);
                        channels[i].SetBlacklevel(0.0f);
                        continue;
                    }

                    //fill channel with values from the client
                    channels[i].SetUsed(true);
                    channels[i].SetValue(m_clients[clientnr].m_lights[light].GetColorValue(color, time));
                    channels[i].SetSpeed(m_clients[clientnr].m_lights[light].GetSpeed());
                    channels[i].SetGamma(m_clients[clientnr].m_lights[light].GetGamma(color));
                    channels[i].SetAdjust(m_clients[clientnr].m_lights[light].GetAdjust(color));
                    channels[i].SetBlacklevel(m_clients[clientnr].m_lights[light].GetBlacklevel(color));
                    channels[i].SetSingleChange(m_clients[clientnr].m_lights[light].GetSingleChange(device));

                    //save pointer to this light because we have to reset the singlechange later
                    //more than one channel can use a light so can't do this from the loop
                    usedlights.Add(m_clients[clientnr].m_lights[light]);
                }

                //remove duplicate lights
                //usedlights.sort();
                //usedlights.unique();

                //reset singlechange
                foreach (var usedLight in usedlights.Distinct())
                    usedLight.ResetSingleChange(device);


                //for (list<CLight*>::iterator it = usedlights.begin(); it != usedlights.end(); it++)
                //    (*it)->ResetSingleChange(device);

                //update which lights we're using
                for (int i = 0; i < m_clients.Count; i++)
                {
                    for (int j = 0; j < m_clients[i].m_lights.Count; j++)
                    {
                        bool lightused = false;
                        foreach (CLight it in usedlights)// list<CLight*>::iterator it = usedlights.begin(); it != usedlights.end(); it++)
                        {
                            if (it == m_clients[i].m_lights[j])
                            {
                                lightused = true;
                                break;
                            }
                        }

                        if (lightused)
                            m_clients[i].m_lights[j].AddUser(device);
                        else
                            m_clients[i].m_lights[j].ClearUser(device);
                    }
                }
            }
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
            else if (messagekey == "set")
            {
                return ParseSet(client, message);
            }
            else if (messagekey == "sync")
            {
                return ParseSync(client);
            }
            else
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
            }

            return true;
        }

        private bool ParseSync(CClient client)
        {
            List<CDevice> users = new List<CDevice>();

            lock (m_mutex)
            {
                //build up a list of devices using this client's input
                for (int i = 0; i < client.m_lights.Count; i++)
                {
                    users.AddRange(client.m_lights[i].GetAllUsers());
                    //for (int j = 0; j < client.m_lights[i].GetNrUsers(); j++)
                    //    users.Add(client.m_lights[i].GetUser(j));
                }
            }

            var distinctUsers = users.Distinct();

            //message all devices
            foreach (CDevice device in distinctUsers)
                device.Sync();

            return true;
        }

        private bool ParseSet(CClient client, CMessage message)
        {
            string messagekey;
            if (!Util.GetWord(ref message.message, out messagekey))
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
            }

            if (messagekey == "priority")
            {
                int priority;
                string strpriority;
                if (!Util.GetWord(ref message.message, out strpriority) || !int.TryParse(strpriority, out priority))
                {
                    Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                    return false;
                }
                lock (m_mutex)
                {
                    client.SetPriority(priority);
                }

                Util.Log($"{client.m_socket.Address}:{client.m_socket.Port} priority set to {client.m_priority}");
            }
            else if (messagekey == "light")
            {
                return ParseSetLight(client, message);
            }
            else
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
            }
            return true;
        }

        private bool ParseSetLight(CClient client, CMessage message)
        {
            string lightname;
            string lightkey;
            int lightnr;

            if (!Util.GetWord(ref message.message, out lightname) || !Util.GetWord(ref message.message, out lightkey) || (lightnr = client.LightNameToInt(lightname)) == -1)
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
            }

            if (lightkey == "rgb")
            {
                float[] rgb = new float[3];
                string value;

                Util.ConvertFloatLocale(ref message.message); //workaround for locale mismatch (, and .)

                for (int i = 0; i < 3; i++)
                {
                    if (!Util.GetWord(ref message.message, out value) || !float.TryParse(value, out rgb[i]))
                    {
                        Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                        return false;
                    }
                }

                lock (m_mutex)
                {
                    client.m_lights[lightnr].SetRgb(rgb, message.time);
                }
            }
            else if (lightkey == "speed")
            {
                float speed;
                string value;

                Util.ConvertFloatLocale(ref message.message); //workaround for locale mismatch (, and .)

                if (!Util.GetWord(ref message.message, out value) || !float.TryParse(value, out speed))
                {
                    Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                    return false;
                }

                lock (m_mutex)
                {
                    client.m_lights[lightnr].SetSpeed(speed);
                }
            }
            else if (lightkey == "interpolation")
            {
                bool interpolation;
                string value;

                //TODO: check for true/false vs y/n
                if (!Util.GetWord(ref message.message, out value) || !bool.TryParse(value, out interpolation))
                {
                    Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                    return false;
                }

                lock (m_mutex)
                {
                    client.m_lights[lightnr].SetInterpolation(interpolation);
                }
            }
            else if (lightkey == "use")
            {
                bool use;
                string value;

                //TODO: check for true/false vs y/n
                if (!Util.GetWord(ref message.message, out value) || !bool.TryParse(value, out use))
                {
                    Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                    return false;
                }

                lock (m_mutex)
                {
                    client.m_lights[lightnr].SetUse(use);
                }
            }
            else if (lightkey == "singlechange")
            {
                float singlechange;
                string value;

                Util.ConvertFloatLocale(ref message.message); //workaround for locale mismatch (, and .)

                if (!Util.GetWord(ref message.message, out value) || !float.TryParse(value, out singlechange))
                {
                    Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                    return false;
                }

                lock (m_mutex)
                {
                    client.m_lights[lightnr].SetSingleChange(singlechange);
                }
            }
            else
            {
                Util.LogError($"{client.m_socket.Address}:{client.m_socket.Port} sent gibberish");
                return false;
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
            CTcpData data = new CTcpData();

            //build up messages by appending to CTcpData
            data.SetData("lights " + client.m_lights.Count + "\n");

            for (int i = 0; i < client.m_lights.Count; i++)
            {
                data.SetData("light " + client.m_lights[i].Name + " ", true);

                data.SetData("scan ", true);
                data.SetData(client.m_lights[i].GetVscan()[0].ToString() + " ", true);
                data.SetData(client.m_lights[i].GetVscan()[1].ToString() + " ", true);
                data.SetData(client.m_lights[i].GetHscan()[0].ToString() + " ", true);
                data.SetData(client.m_lights[i].GetHscan()[1].ToString(), true);
                data.SetData("\n", true);
            }

            if (client.m_socket.Write(data) != true)
            {
                Util.Log(client.m_socket.GetError());
                return false;
            }
            return true;
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
            lock (m_mutex)
            {
                //TODO: can use linq to remove the for loop/if statement
                for (int i = 0; i < m_clients.Count; i++)
                {
                    if (m_clients[i].m_socket.GetSock() == client.m_socket.GetSock())
                    {
                        Util.Log($"removing {m_clients[i].m_socket.Address}:{m_clients[i].m_socket.Port}");
                        m_clients[i].Dispose();
                        m_clients.RemoveAt(i);
                        return;
                    }
                }
            }
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