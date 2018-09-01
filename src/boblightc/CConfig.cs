using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;

namespace boblightc
{
    class CConfig
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

        public CConfig()
        {
            m_globalconfiglines = new List<CConfigLine>();
            m_devicelines = new List<CConfigGroup>();
            m_colorlines = new List<CConfigGroup>();
            m_lightlines = new List<CConfigGroup>();
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

        internal bool CheckConfig()
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
                        if (!bool.TryParse(value, out bValue))
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
                    { //these are floats from 0.0 to 1.0, except gamma which is from 0.0 and up
                        float fvalue;
                        if (!float.TryParse(value, out fvalue) || fvalue < 0.0 || (key != "gamma" && fvalue > 1.0))
                        {
                            Util.LogError($"{m_filename} line {linenr} section [color]: wrong value {value} for key {key}");
                            valid = false;
                        }
                    }
                    else if (key == "rgb")
                    { //rgb lines in hex notation, like: rgb FF0000 for red and rgb 0000FF for blue
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
                        bool parsedValueOne = float.TryParse(scanRangePieces[0], out scan[0]);
                        bool parsedValueTwo = float.TryParse(scanRangePieces[1], out scan[1]);

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
