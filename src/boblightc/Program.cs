using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace boblightc
{
    class Program
    {
        static void Main(string[] args)
        {
            //read flags
            string configfile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "boblightc.conf");
            bool help;
            bool bfork;

            //if (!ParseFlags(argc, argv, help, configfile, bfork) || help)
            //{
            //    PrintHelpMessage();
            //    return 1;
            //}

            //if (bfork)
            //{
            //    Daemonize();
            //    logtostderr = false;
            //}

            //init our logfile
            //SetLogFile("boblightd.log");
            //PrintFlags(argc, argv);

            //set up signal handlers
            //signal(SIGTERM, SignalHandler);
            //signal(SIGINT, SignalHandler);

            List<CDevice> devices = new List<CDevice>(); //where we store devices
            List<CLight> lights = new List<CLight>();  //lights pool
            CClientsHandler clients = new CClientsHandler(lights);

            { //save some ram by removing CConfig from the stack when it's not needed anymore
                CConfig config = new CConfig();

                //load and parse config
                if (!config.LoadConfigFromFile(configfile))
                    return; // failed
                if (!config.CheckConfig())
                    return;
                if (!config.BuildConfig(clients, devices, lights))
                    return;
            }

            //start the devices
            Util.Log("starting devices");
            //for (int i = 0; i < devices.Count; i++)
            //    devices[i].StartThread();

            //run the clients handler
            bool g_stop = false; //TODO: wait for console keypress
            //while (!g_stop)
                clients.Process();

            //signal that the devices should stop
            //Log("signaling devices to stop");
            //for (int i = 0; i < devices.size(); i++)
            //    devices[i]->AsyncStopThread();

            //clean up the clients handler
            clients.Cleanup();

            //stop the devices
            //Log("waiting for devices to stop");
            //for (int i = 0; i < devices.size(); i++)
            //    devices[i]->StopThread();

            Util.Log("exiting");


        }
    }
}
