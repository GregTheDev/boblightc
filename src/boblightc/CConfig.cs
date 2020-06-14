using boblightc.Device;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace boblightc
{
    public class CConfig
    {
        private const int SECTNOTHING = 0;
        private const int SECTGLOBAL = 1;
        private const int SECTDEVICE = 2;
        private const int SECTCOLOR = 3;
        private const int SECTLIGHT = 4;

        private string m_filename;
        private List<CConfigLine> m_globalconfiglines;
        private List<CConfigGroup> m_devicelines;
        private List<CConfigGroup> m_colorlines;
        private List<CConfigGroup> m_lightlines;

        private NumberFormatInfo _configFileNumberFormat;

        public CConfig()
        {
            m_globalconfiglines = new List<CConfigLine>();
            m_devicelines = new List<CConfigGroup>();
            m_colorlines = new List<CConfigGroup>();
            m_lightlines = new List<CConfigGroup>();

            _configFileNumberFormat = new NumberFormatInfo();
            _configFileNumberFormat.NumberDecimalSeparator = "."; // No support for culture specific number formats in config files for now
        }

        public bool LoadConfigFromFile(string file)
        {
            int linenr = 0;
            int currentsection = SECTNOTHING;

            m_filename = file;

            Util.Log($"opening {file}");

            //try to open the config file
            //TODO: switch to using statement
            StreamReader configfile;

            try
            {
                configfile = File.OpenText(file);
            }
            catch (Exception ex)
            {
                Util.LogError($"{file}: {ex.ToString()}");

                return false;
            }

            try
            {
                //read lines from the config file and store them in the appropriate sections
                while (!configfile.EndOfStream)
                {
                    string buff = configfile.ReadLine();

                    linenr++;
                    //if (configfile.fail())
                    //{
                    //    if (!configfile.eof())
                    //        LogError("reading %s line %i: %s", file.c_str(), linenr, GetErrno().c_str());
                    //    break;
                    //}

                    string line = buff;
                    string key = null;
                    //if the line doesn't have a word it's not important
                    if (!Util.GetWord(ref line, out key))
                        continue;

                    //ignore comments
                    if (key[0] == '#')
                        continue;

                    //check if we entered a section
                    if (key == "[global]")
                    {
                        currentsection = SECTGLOBAL;
                        continue;
                    }
                    else if (key == "[device]")
                    {
                        currentsection = SECTDEVICE;
                        m_devicelines.Add(new CConfigGroup());
                        continue;
                    }
                    else if (key == "[color]")
                    {
                        currentsection = SECTCOLOR;
                        m_colorlines.Add(new CConfigGroup());
                        continue;
                    }
                    else if (key == "[light]")
                    {
                        currentsection = SECTLIGHT;
                        m_lightlines.Add(new CConfigGroup());
                        continue;
                    }

                    //we're not in a section
                    if (currentsection == SECTNOTHING)
                        continue;

                    CConfigLine configline = new CConfigLine(buff, linenr);

                    //store the config line in the appropriate section
                    if (currentsection == SECTGLOBAL)
                    {
                        m_globalconfiglines.Add(configline);
                    }
                    else if (currentsection == SECTDEVICE)
                    {
                        m_devicelines.Last().lines.Add(configline);
                    }
                    else if (currentsection == SECTCOLOR)
                    {
                        m_colorlines.Last().lines.Add(configline);
                    }
                    else if (currentsection == SECTLIGHT)
                    {
                        m_lightlines.Last().lines.Add(configline);
                    }
                }
            }
            finally
            {
                configfile.Dispose();
            }

            return true;
        }

        public bool BuildConfig(IServerConfiguration clients, IChannelDataProvider channelDataProvider, List<CDevice> devices, List<CLight> lights)
        {
            Util.Log("building config");

            BuildClientsHandlerConfig(clients);

            List<CColor> colors = new List<CColor>();
            if (!BuildColorConfig(colors))
                return false;

            if (!BuildDeviceConfig(devices, channelDataProvider))
                return false;

            if (!BuildLightConfig(lights, devices, colors))
                return false;

            Util.Log("built config successfully");

            return true;
        }

        private void BuildClientsHandlerConfig(IServerConfiguration clients)
        {
            //set up where to bind the listening socket
            //config for this should already be valid here, of course we can't check yet if the interface actually exists
            string interfaceAddress = String.Empty; //empty string means bind to *
            int port = 19333; //default port
            for (int i = 0; i < m_globalconfiglines.Count; i++)
            {
                string line = m_globalconfiglines[i].line;
                string word;
                Util.GetWord(ref line, out word);

                if (word == "interface")
                {
                    Util.GetWord(ref line, out interfaceAddress);
                }
                else if (word == "port")
                {
                    Util.GetWord(ref line, out word);
                    port = int.Parse(word);
                }
            }
            clients.SetInterface(interfaceAddress, port);
        }

        private bool BuildColorConfig(List<CColor> colors)
        {
            for (int i = 0; i < m_colorlines.Count; i++)
            {
                CColor color = new CColor();
                for (int j = 0; j < m_colorlines[i].lines.Count; j++)
                {
                    string line = m_colorlines[i].lines[j].line;
                    string key, value;

                    Util.GetWord(ref line, out key);
                    Util.GetWord(ref line, out value);

                    if (key == "name")
                    {
                        color.Name = value;
                    }
                    else if (key == "rgb")
                    {
                        int irgb;
                        float[] frgb = new float[3];

                        irgb = int.Parse(value, NumberStyles.HexNumber, null);

                        for (int k = 0; k < 3; k++)
                            frgb[k] = (float)(((irgb >> ((2 - k) * 8)) & 0xFF) / 255.0);

                        color.Rgb = frgb;
                    }
                    else if (key == "gamma")
                    {
                        float gamma;
                        Util.ConvertFloatLocale(ref value);
                        gamma = float.Parse(value);
                        color.m_gamma = gamma;
                    }
                    else if (key == "adjust")
                    {
                        float adjust;
                        Util.ConvertFloatLocale(ref value);
                        adjust = float.Parse(value);
                        color.m_adjust = adjust;
                    }
                    else if (key == "blacklevel")
                    {
                        float blacklevel;
                        Util.ConvertFloatLocale(ref value);
                        blacklevel = float.Parse(value);
                        color.m_blacklevel = blacklevel;
                    }
                }

                //we need at least a name for a color
                if (String.IsNullOrEmpty(color.Name))
                {
                    Util.LogError($"{m_filename}: color {i + 1} has no name");
                    return false;
                }

                colors.Add(color);
            }

            return true;
        }

        private bool BuildLightConfig(List<CLight> lights, List<CDevice> devices, List<CColor> colors)
        {
            for (int i = 0; i < m_lightlines.Count; i++)
            {
                CLight light = new CLight();

                if (!SetLightName(light, m_lightlines[i].lines, i))
                    return false;

                SetLightScanRange(light, m_lightlines[i].lines);

                //check the colors on a light
                for (int j = 0; j < m_lightlines[i].lines.Count; j++)
                {
                    string line = m_lightlines[i].lines[j].line;
                    string key;
                    Util.GetWord(ref line, out key);

                    if (key != "color")
                        continue;

                    //we already checked these in the syntax check
                    string colorname, devicename, devicechannel;
                    Util.GetWord(ref line, out colorname);
                    Util.GetWord(ref line, out devicename);
                    Util.GetWord(ref line, out devicechannel);

                    bool colorfound = false;
                    for (int k = 0; k < colors.Count; k++)
                    {
                        if (colors[k].Name == colorname)
                        {
                            colorfound = true;
                            light.AddColor(colors[k]);
                            break;
                        }
                    }
                    if (!colorfound) //this color doesn't exist
                    {
                        Util.LogError($"{m_filename} line {m_lightlines[i].lines[j].linenr}: no color with name {colorname}");
                        return false;
                    }

                    int ichannel;
                    ichannel = int.Parse(devicechannel);

                    //loop through the devices, check if one with this name exists and if the channel on it exists
                    bool devicefound = false;
                    for (int k = 0; k < devices.Count; k++)
                    {
                        if (devices[k].Name == devicename)
                        {
                            if (ichannel > devices[k].NrChannels)
                            {
                                Util.LogError($"{m_filename} line {m_lightlines[i].lines[j].linenr}: channel {ichannel} wanted but device {devices[k].Name} has {devices[k].NrChannels} channels");
                                return false;
                            }
                            devicefound = true;
                            CChannel channel = new CChannel();
                            channel.Color = (light.NrColors - 1);
                            channel.Light = i;
                            devices[k].SetChannel(channel, ichannel - 1);
                            break;
                        }
                    }
                    if (!devicefound)
                    {
                        Util.LogError($"{m_filename} line {m_lightlines[i].lines[j].linenr}: no device with name {devicename}");
                        return false;
                    }
                }
                lights.Add(light);
            }

            return true;
        }

        private void SetLightScanRange(CLight light, List<CConfigLine> lines)
        {
            //hscan and vdscan are optional

            string line, strvalue;
            int linenr = GetLineWithKey("hscan", lines, out line);
            if (linenr != -1)
            {
                float[] hscan = new float[2];
                string[] linePieces = line.Split(' ');
                hscan[0] = float.Parse(linePieces[0], _configFileNumberFormat);
                hscan[1] = float.Parse(linePieces[1], _configFileNumberFormat);

                //sscanf(line.c_str(), "%f %f", hscan, hscan + 1);
                light.SetHscan(hscan);
            }

            linenr = GetLineWithKey("vscan", lines, out line);
            if (linenr != -1)
            {
                float[] vscan = new float[2];
                string[] linePieces = line.Split(' ');
                vscan[0] = float.Parse(linePieces[0], _configFileNumberFormat);
                vscan[1] = float.Parse(linePieces[1], _configFileNumberFormat);

                //sscanf(line.c_str(), "%f %f", vscan, vscan + 1);
                light.SetVscan(vscan);
            }
        }

        private bool SetLightName(CLight light, List<CConfigLine> lines, int lightnr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("name", lines, out line);
            if (linenr == -1)
            {
                Util.LogError($"{m_filename}: light {lightnr + 1} has no name");
                return false;
            }

            Util.GetWord(ref line, out strvalue);
            light.Name = strvalue;
            return true;
        }

        private bool BuildDeviceConfig(List<CDevice> devices, IChannelDataProvider clients)
        {
            for (int i = 0; i < m_devicelines.Count; i++)
            {
                string line;
                int linenr = GetLineWithKey("type", m_devicelines[i].lines, out line);
                string type;

                Util.GetWord(ref line, out type);

                if (type == "popen")
                {
                    throw new NotImplementedException("'popen' devices not supported yet");
                    //CDevice device = null;
                    //if (!BuildPopen(device, i, clients))
                    //{
                    //    if (device)
                    //        delete device;
                    //    return false;
                    //}
                    //devices.push_back(device);
                }
                else if (type == "momo" || type == "atmo" || type == "karate" || type == "sedu")
                {
                    CDevice device = null;
                    if (!BuildRS232(out device, i, clients, type))
                    {
                        if (device != null)
                            device = null; //TODO: Call dispose instead???
                        return false;
                    }
                    devices.Add(device);
                }
                else if (type == "ltbl")
                {
                    throw new NotImplementedException("'ltbl' devices not supported yet");

                    //CDevice* device = NULL;
                    //if (!BuildLtbl(device, i, clients))
                    //{
                    //    if (device)
                    //        delete device;
                    //    return false;
                    //}
                    //devices.push_back(device);
                }
                else if (type == "ola")
                {
                    throw new NotImplementedException("'ola' devices not supported yet");

                    //# ifdef HAVE_OLA
                    //                    CDevice* device = NULL;
                    //                    if (!BuildOla(device, i, clients))
                    //                    {
                    //                        if (device)
                    //                            delete device;
                    //                        return false;
                    //                    }
                    //                    devices.push_back(device);
                    //#else
                    //                    LogError("%s line %i: boblightd was built without ola, recompile with enabled ola support",
                    //                             m_filename.c_str(), linenr);
                    //                    return false;
                    //#endif
                }
                else if (type == "sound")
                {
                    throw new NotImplementedException("'sound' devices not supported yet");

                    //# ifdef HAVE_LIBPORTAUDIO
                    //                    CDevice* device = NULL;
                    //                    if (!BuildSound(device, i, clients))
                    //                    {
                    //                        if (device)
                    //                            delete device;
                    //                        return false;
                    //                    }
                    //                    devices.push_back(device);
                    //#else
                    //                    LogError("%s line %i: boblightd was built without portaudio, no support for sound devices",
                    //                             m_filename.c_str(), linenr);
                    //                    return false;
                    //#endif
                }
                else if (type == "dioder")
                {
                    throw new NotImplementedException("'dioder' devices not supported yet");

                    //CDevice* device = NULL;
                    //if (!BuildDioder(device, i, clients))
                    //{
                    //    if (device)
                    //        delete device;
                    //    return false;
                    //}
                    //devices.push_back(device);
                }
                else if (type == "ambioder")
                {
                    throw new NotImplementedException("'ambioder' devices not supported yet");

                    //CDevice* device = NULL;
                    //if (!BuildAmbioder(device, i, clients))
                    //{
                    //    if (device)
                    //        delete device;
                    //    return false;
                    //}
                    //devices.push_back(device);
                }
                else if (type == "ibelight")
                {
                    throw new NotImplementedException("'ibelight' devices not supported yet");

                    //# ifdef HAVE_LIBUSB
                    //                    CDevice* device = NULL;
                    //                    if (!BuildiBeLight(device, i, clients))
                    //                    {
                    //                        if (device)
                    //                            delete device;
                    //                        return false;
                    //                    }
                    //                    devices.push_back(device);
                    //#else
                    //                    LogError("%s line %i: boblightd was built without libusb, no support for ibelight devices",
                    //                             m_filename.c_str(), linenr);
                    //                    return false;
                    //#endif
                }
                else if (type == "lightpack")
                {
                    throw new NotImplementedException("'lightpack' devices not supported yet");

                    //# ifdef HAVE_LIBUSB
                    //                    CDevice* device = NULL;
                    //                    if (!BuildLightpack(device, i, clients))
                    //                    {
                    //                        if (device)
                    //                            delete device;
                    //                        return false;
                    //                    }
                    //                    devices.push_back(device);
                    //#else
                    //                    LogError("%s line %i: boblightd was built without libusb, no support for lightpack devices",
                    //                             m_filename.c_str(), linenr);
                    //                    return false;
                    //#endif
                }
                else if (type == "lpd8806" || type == "ws2801")
                {
                    throw new NotImplementedException("'lpd8806' and 'ws2801' devices are not supported yet");

                    //# ifdef HAVE_LINUX_SPI_SPIDEV_H
                    //                    CDevice* device = NULL;
                    //                    if (!BuildSPI(device, i, clients, type))
                    //                    {
                    //                        if (device)
                    //                            delete device;
                    //                        return false;
                    //                    }
                    //                    devices.push_back(device);
                    //#else
                    //                    LogError("%s line %i: boblightd was built without spi, no support for lpd8806 devices",
                    //                             m_filename.c_str(), linenr);
                    //                    return false;
                    //#endif
                }
                else
                {
                    Util.LogError($"{m_filename} line {linenr}: unknown type {type}");
                    return false;
                }
            }

            return true;
        }

        private bool BuildRS232(out CDevice device, int devicenr, IChannelDataProvider clients, string type)
        {
            CDeviceRS232 rs232device = new CDeviceRS232(clients);
            device = rs232device;

            if (!SetDeviceName(rs232device, devicenr))
                return false;

            if (!SetDeviceOutput(rs232device, devicenr))
                return false;

            if (!SetDeviceChannels(rs232device, devicenr))
                return false;

            if (!SetDeviceRate(rs232device, devicenr))
                return false;

            if (!SetDeviceInterval(rs232device, devicenr))
                return false;

            SetDeviceAllowSync(device, devicenr);
            SetDeviceDebug(device, devicenr);
            SetDeviceDelayAfterOpen(device, devicenr);
            SetDeviceThreadPriority(device, devicenr);

            bool hasbits = SetDeviceBits(rs232device, devicenr);
            bool hasmax = SetDeviceMax(rs232device, devicenr);
            if (hasbits && hasmax)
            {
                Util.LogError($"{m_filename}: device {rs232device.Name} has both bits and max set");
                return false;
            }

            if (type == "momo")
            {
                device.Type = CDevice.MOMO;
                SetDevicePrefix(rs232device, devicenr);
                SetDevicePostfix(rs232device, devicenr);
            }
            else if (type == "atmo")
            {
                device.Type = CDevice.ATMO;
            }
            else if (type == "karate")
            {
                device.Type = CDevice.KARATE;
            }
            else if (type == "sedu")
            {
                device.Type = CDevice.SEDU;
            }

            return true;
        }

        private bool SetDeviceName(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("name", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
            {
                Util.LogError($"{m_filename}: device {devicenr + 1} has no name");
                return false;
            }
            Util.GetWord(ref line, out strvalue);
            device.Name = strvalue;

            return true;
        }

        private bool SetDeviceOutput(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("output", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
            {
                Util.LogError($"{m_filename}: device {device.Name} has no output");
                return false;
            }
            Util.GetWord(ref line, out strvalue);
            device.Output = (strvalue + line);

            return true;
        }

        private bool SetDeviceChannels(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("channels", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
            {
                Util.LogError($"{m_filename}: device {device.Name} has no channels");
                return false;
            }
            Util.GetWord(ref line, out strvalue);

            int nrchannels;
            nrchannels = int.Parse(strvalue);
            device.NrChannels = nrchannels;

            return true;
        }

        private bool SetDeviceRate(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("rate", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
            {
                Util.LogError($"{m_filename}: device {device.Name} has no rate");
                return false;
            }
            Util.GetWord(ref line, out strvalue);

            int rate;
            rate = int.Parse(strvalue);
            device.Rate = rate;

            return true;
        }

        private bool SetDeviceInterval(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("interval", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
            {
                Util.LogError($"{m_filename}: device {device.Name} has no interval");
                return false;
            }
            Util.GetWord(ref line, out strvalue);

            int interval;
            interval = int.Parse(strvalue);
            device.Interval = interval;

            return true;
        }

        private void SetDevicePrefix(CDevice device, int devicenr)
        {
            string line, strvalue;
            List<byte> prefix = new List<byte>();
            int linenr = GetLineWithKey("prefix", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
            {
                return; //prefix is optional, so this is not an error 
            }

            while (Util.GetWord(ref line, out strvalue))
            {
                byte iprefix;
                iprefix = byte.Parse(strvalue, NumberStyles.HexNumber, null);
                prefix.Add(iprefix);
            }
            device.m_prefix = prefix;
        }

        private void SetDevicePostfix(CDevice device, int devicenr)
        {
            string line, strvalue;
            List<byte> postfix = new List<byte>(); //TODO, confirm size of type uint8_t
            int linenr = GetLineWithKey("postfix", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
            {
                return; //postfix is optional, so this is not an error 
            }

            while (Util.GetWord(ref line, out strvalue))
            {
                byte ipostfix;
                ipostfix = byte.Parse(strvalue, NumberStyles.HexNumber, null);
                postfix.Add(ipostfix);
            }
            device.m_postfix = postfix;
        }

        private void SetDeviceAllowSync(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("allowsync", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
                return;

            Util.GetWord(ref line, out strvalue);

            Util.StrToBool(strvalue, out bool allowsync);
            device.AllowSync = allowsync;
        }

        private void SetDeviceDebug(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("debug", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
                return;

            Util.GetWord(ref line, out strvalue);

            Util.StrToBool(strvalue, out bool debug);
            device.Debug = debug;
        }

        private bool SetDeviceBits(CDeviceRS232 device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("bits", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
                return false;

            Util.GetWord(ref line, out strvalue);

            int bits;
            bits = int.Parse(strvalue);
            device.m_max = (1 << bits) - 1; //TODO: verify ((1 << (int64_t)bits) - 1);

            return true;
        }

        private bool SetDeviceMax(CDeviceRS232 device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("max", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
                return false;

            Util.GetWord(ref line, out strvalue);

            Int64 max;
            max = Int64.Parse(strvalue);
            device.m_max = max;

            return true;
        }

        private void SetDeviceDelayAfterOpen(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("delayafteropen", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
                return;

            Util.GetWord(ref line, out strvalue);

            int delayafteropen;
            delayafteropen = int.Parse(strvalue);
            device.DelayAfterOpen = delayafteropen;
        }

        private void SetDeviceThreadPriority(CDevice device, int devicenr)
        {
            string line, strvalue;
            int linenr = GetLineWithKey("threadpriority", m_devicelines[devicenr].lines, out line);
            if (linenr == -1)
                return;

            Util.GetWord(ref line, out strvalue);

            int priority;
            priority = int.Parse(strvalue); // TODO: windows priority vs unix priority? do old configs line up???
            device.ThreadPriority = (ThreadPriority) priority;
        }

        private int GetLineWithKey(string key, List<CConfigLine> lines, out string line)
        {
            line = String.Empty; // null??

            for (int i = 0; i < lines.Count; i++)
            {
                string word;
                line = lines[i].line;
                Util.GetWord(ref line, out word);

                if (word == key)
                {
                    return lines[i].linenr;
                }
            }

            return -1;
        }

        public bool CheckConfig()
        {
            bool valid = true;
            Util.Log("checking config lines");

            if (!CheckGlobalConfig())
                valid = false;

            if (!CheckDeviceConfig())
                valid = false;

            if (!CheckColorConfig())
                valid = false;

            if (!CheckLightConfig())
                valid = false;

            if (valid)
                Util.Log("config lines valid");

            return valid;
        }

        private bool CheckGlobalConfig()
        {
            bool valid = true;

            for (int i = 0; i < m_globalconfiglines.Count; i++)
            {
                string line = m_globalconfiglines[i].line;
                string key;
                string value;

                Util.GetWord(ref line, out key); //we already checked each line starts with one word

                if (!Util.GetWord(ref line, out value)) //every line here needs to have another word
                {
                    Util.LogError($"{m_filename} line {m_globalconfiglines[i].linenr} section [global]: no value for key {key}");
                    valid = false;
                    continue;
                }

                if (key == "interface")
                {
                    continue; //not much to check here
                }
                else if (key == "port") //check tcp/ip port
                {
                    int port;
                    bool parsedPort = Int32.TryParse(value, out port);

                    if (!parsedPort || port < 0 || port > 65535)
                    {
                        Util.LogError($"{m_filename} line {m_globalconfiglines[i].linenr} section [global]: wrong value {value} for key {key}");
                        valid = false;
                    }
                }
                else //we don't know this one
                {
                    Util.LogError($"{m_filename} line {m_globalconfiglines[i].linenr} section [global]: unknown key {key}");
                    valid = false;
                }
            }

            return valid;
        }

        private bool CheckDeviceConfig()
        {
            bool valid = true;

            if (m_devicelines.Count == 0)
            {
                Util.LogError($"{m_filename} no devices defined");
                return false;
            }

            for (int i = 0; i < m_devicelines.Count; i++)
            {
                for (int j = 0; j < m_devicelines[i].lines.Count; j++)
                {
                    string line = m_devicelines[i].lines[j].line;
                    int linenr = m_devicelines[i].lines[j].linenr;
                    string key;
                    string value;

                    Util.GetWord(ref line, out key); //we already checked each line starts with one word

                    if (!Util.GetWord(ref line, out value)) //every line here needs to have another word
                    {
                        Util.LogError($"{m_filename} line {linenr} section [device]: no value for key {key}");
                        valid = false;
                        continue;
                    }

                    if (key == "name" || key == "output" || key == "type" || key == "serial")
                    {
                        continue; //can't check these here
                    }
                    else if (key == "rate" || key == "channels" || key == "interval" || key == "period" ||
                             key == "bits" || key == "delayafteropen" || key == "max" || key == "precision")
                    { //these are of type integer not lower than 1
                        long ivalue;
                        bool parsedValue = long.TryParse(value, out ivalue);
                        if (!parsedValue || ivalue < 1 || (key == "bits" && ivalue > 32) || (key == "max" && ivalue > 0xFFFFFFFF))
                        {
                            Util.LogError($"{m_filename} line {linenr} section [device]: wrong value {value} for key {key}");
                            valid = false;
                        }
                    }
                    else if (key == "threadpriority")
                    {
                        long ivalue;
                        if (!long.TryParse(value, out ivalue))
                        {
                            Util.LogError($"{m_filename} line {linenr} section [device]: wrong value {value} for key {key}");
                            valid = false;
                        }


                        int priomax = (int) System.Threading.ThreadPriority.Highest; // sched_get_priority_max(SCHED_FIFO);
                        int priomin = (int)System.Threading.ThreadPriority.Lowest; // sched_get_priority_min(SCHED_FIFO);
                        if (ivalue > priomax || ivalue < priomin)
                        {
                            Util.LogError($"{m_filename} line {linenr} section [device]: threadpriority must be between {priomin} and {priomax}");
                            valid = false;
                        }
                    }
                    else if (key == "prefix" || key == "postfix")
                    { //this is in hex from 00 to FF, separated by spaces, like: prefix FF 7F A0 22
                        int ivalue;
                        bool parsedValue;

                        while (true)
                        {
                            parsedValue = Int32.TryParse(value, NumberStyles.HexNumber, null, out ivalue);

                            if (!parsedValue || (ivalue > 0xFF))
                            {
                                Util.LogError($"{m_filename} line {linenr} section [device]: wrong value {value} for key {key}");
                                valid = false;
                            }
                            if (!Util.GetWord(ref line, out value))
                                break;
                        }
                    }
                    else if (key == "latency")//this is a double
                    {
                        double latency;
                        if (!double.TryParse(value, out latency) || latency <= 0.0)
                        {
                            Util.LogError("{m_filename} line {linenr} section [device]: wrong value {value} for key {key}");
                            valid = false;
                        }
                    }
                    else if (key == "allowsync" || key == "debug")//bool
                    {
                        bool bValue;
                        if (!Util.StrToBool(value, out bValue))
                        {
                            if (key == "debug" && (value == "on" || value == "off")) break; //TODO: sample config has on/off not true/false

                            Util.LogError("{m_filename} line {linenr} section [device]: wrong value {value} for key {key}");
                            valid = false;
                        }
                    }
                    else if (key == "bus" || key == "address") //int, 0 to 255
                    {
                        int ivalue;
                        if (!Int32.TryParse(value, out ivalue) || ivalue < 0 || ivalue > 255)
                        {
                            Util.LogError("{m_filename} line {linenr} section [device]: wrong value {value} for key {key}");
                            valid = false;
                        }
                    }
                    else //don't know this one
                    {
                        Util.LogError($"{m_filename} line {linenr} section [device]: unknown key {key}");
                        valid = false;
                    }
                }
            }

            return valid;
        }

        private bool CheckColorConfig()
        {
            bool valid = true;

            if (m_colorlines.Count == 0)
            {
                Util.LogError($"{m_filename} no colors defined");
                return false;
            }

            for (int i = 0; i < m_colorlines.Count; i++)
            {
                for (int j = 0; j < m_colorlines[i].lines.Count; j++)
                {
                    string line = m_colorlines[i].lines[j].line;
                    int linenr = m_colorlines[i].lines[j].linenr;
                    string key;
                    string value;

                    Util.GetWord(ref line, out key); //we already checked each line starts with one word

                    if (!Util.GetWord(ref line, out value)) //every line here needs to have another word
                    {
                        Util.LogError($"{m_filename} line {linenr} section [color]: no value for key {key}");
                        valid = false;
                        continue;
                    }

                    if (key == "name")
                    {
                        continue;
                    }
                    else if (key == "gamma" || key == "adjust" || key == "blacklevel")
                    { 
                        //these are floats from 0.0 to 1.0, except gamma which is from 0.0 and up
                        float fvalue;
                        if (!float.TryParse(value, NumberStyles.Float, _configFileNumberFormat, out fvalue) || fvalue < 0.0 || (key != "gamma" && fvalue > 1.0))
                        {
                            Util.LogError($"{m_filename} line {linenr} section [color]: wrong value {value} for key {key}");
                            valid = false;
                        }
                    }
                    else if (key == "rgb")
                    { 
                        //rgb lines in hex notation, like: rgb FF0000 for red and rgb 0000FF for blue
                        int rgb;
                        bool parsedValue = Int32.TryParse(value, NumberStyles.HexNumber, null, out rgb);

                        if (!parsedValue || ((rgb & 0xFF000000) != 0))
                        {
                            Util.LogError($"{m_filename} line {linenr} section [color]: wrong value {value} for key {key}");
                            valid = false;
                        }
                    }
                    else //don't know this one
                    {
                        Util.LogError($"{m_filename} line {linenr} section [color]: unknown key {key}");
                        valid = false;
                    }
                }
            }

            return valid;
        }

        private bool CheckLightConfig()
        {
            bool valid = true;

            if (m_lightlines.Count == 0)
            {
                Util.LogError($"{m_filename} no lights defined");
                return false;
            }

            for (int i = 0; i < m_lightlines.Count; i++)
            {
                for (int j = 0; j < m_lightlines[i].lines.Count; j++)
                {
                    string line = m_lightlines[i].lines[j].line;
                    int linenr = m_lightlines[i].lines[j].linenr;
                    string key;
                    string value;

                    Util.GetWord(ref line, out key); //we already checked each line starts with one word

                    if (!Util.GetWord(ref line, out value)) //every line here needs to have another word
                    {
                        Util.LogError("{m_filename} line {linenr} section [light]: no value for key {key}");
                        valid = false;
                        continue;
                    }

                    if (key == "name")
                    {
                        continue;
                    }
                    else if (key == "vscan" || key == "hscan") //check the scanrange, should be two floats from 0.0 to 100.0
                    {
                        string scanrange;
                        float[] scan = new float[2];

                        if (!Util.GetWord(ref line, out scanrange))
                        {
                            Util.LogError($"{m_filename} line {linenr} section [light]: not enough values for key {key}");
                            valid = false;
                            continue;
                        }

                        scanrange = value + " " + scanrange;
                        string[] scanRangePieces = scanrange.Split(' ');
                        bool parsedValueOne = float.TryParse(scanRangePieces[0], NumberStyles.Float | NumberStyles.AllowThousands, _configFileNumberFormat, out scan[0]);
                        bool parsedValueTwo = float.TryParse(scanRangePieces[1], NumberStyles.Float | NumberStyles.AllowThousands, _configFileNumberFormat, out scan[1]);

                        if (!parsedValueOne || !parsedValueTwo || scan[0] < 0.0 || scan[0] > 100.0 || scan[1] < 0.0 || scan[1] > 100.0 || scan[0] > scan[1])
                        {
                            Util.LogError($"{m_filename} line {linenr} section [light]: wrong value {scanrange} for key {key}");
                            valid = false;
                            continue;
                        }
                    }
                    else if (key == "color")
                    { //describes a color for a light, on a certain channel of a device
                      //we can only check if it has enough keys and channel is a positive int from 1 or higher
                        int k;
                        for (k = 0; k < 2; k++)
                        {
                            if (!Util.GetWord(ref line, out value))
                            {
                                Util.LogError($"{m_filename} line {linenr} section [light]: not enough values for key {key}");
                                valid = false;
                                break;
                            }
                        }
                        if (k == 2)
                        {
                            int channel;
                            if (!Int32.TryParse(value, out channel) || channel <= 0)
                            {
                                Util.LogError($"{m_filename} line {linenr} section [light]: wrong value {value} for key {key}");
                                valid = false;
                            }
                        }
                    }
                    else //don't know this one
                    {
                        Util.LogError($"{m_filename} line {linenr} section [light]: unknown key {key}");
                        valid = false;
                    }
                }
            }

            return valid;
        }

    }
}
