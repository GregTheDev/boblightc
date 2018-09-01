using System;

namespace boblightc
{
    internal class Util
    {

        internal static void LogError(string v)
        {
            //TODO: Provide implementation
        }

        internal static void Log(string v)
        {
            //TODO: Provide implementation
        }

        internal static bool GetWord(ref string data, out string word)
        {
            word = string.Empty;
            //stringstream datastream(data);
            string end;

            int endOfToken = data.IndexOf(' ');
            if (endOfToken == -1) endOfToken = (data.Length > 0) ? data.Length : -1;

            if (endOfToken == -1)
            {
                data = String.Empty;
                return false;
            }

            word = data.Substring(0, endOfToken);

            //size_t pos = data.find(word) + word.length();
            int pos = data.IndexOf(word) + word.Length; //TODO: makes no sense? word is always the first char?

            if (pos >= data.Length)
            {
                data = String.Empty;
                return true;
            }

            data = data.Substring(pos);

            //TODO: Not sure why this is here
            //datastream.clear();
            //datastream.str(data);

            //datastream >> end;
            //if (datastream.fail())
            //    data.clear();

            return true;
        }
    }
}