using log4net;
using log4net.Config;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace boblightc
{
    internal class Util
    {
        private static ILog _logger;
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1);

        static Util()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        internal static void LogError(string message)
        {
            _logger.Error(message);
        }

        internal static void Log(string message)
        {
            _logger.Debug(message);
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

            data = data.Trim();

            //TODO: Not sure why this is here
            //datastream.clear();
            //datastream.str(data);

            //datastream >> end;
            //if (datastream.fail())
            //    data.clear();

            return true;
        }

        internal static void ConvertFloatLocale(ref string value)
        {
            string localDecimalSeperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            value.Replace(",", localDecimalSeperator);
            value.Replace(".", localDecimalSeperator);
        }

        internal static void ConvertYesNoToTrueFalse(ref string value)
        {
            value.Replace("y", "true", StringComparison.InvariantCultureIgnoreCase);
            value.Replace("n", "false", StringComparison.InvariantCultureIgnoreCase);
            value.Replace("yes", "true", StringComparison.InvariantCultureIgnoreCase);
            value.Replace("no", "false", StringComparison.InvariantCultureIgnoreCase);
        }

        internal static long GetTimeUs()
        {
            // From: https://stackoverflow.com/questions/4856659/is-there-an-equivalent-of-gettimeofday-in-net
            TimeSpan timeSinceEpoch = DateTime.UtcNow - UnixEpoch;

            return timeSinceEpoch.Ticks / 10; // Tick is 100ns
        }
    }
}