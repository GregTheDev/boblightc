using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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
    }
}
