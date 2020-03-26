using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc
{
    public class CMessageQueue
    {
        public static readonly int MAXDATA = 100000;

        private CMessage m_remainingdata;
        private List<CMessage> m_messages;

        public CMessageQueue()
        {
            m_remainingdata = new CMessage();
            m_messages = new List<CMessage>();
        }

        internal void AddData(byte[] data, int size)
        {
            string strdata = Encoding.ASCII.GetString(data);

            AddData(strdata);
        }

        private void AddData(string data)
        {
            long now = DateTime.Now.Ticks;// GetTimeUs();
            int nlpos = data.IndexOf('\n'); //position of the newline

            //no newline
            if (nlpos == -1)
            {
                //set the timestamp if there's no remaining data
                if (string.IsNullOrEmpty(m_remainingdata.message))
                    m_remainingdata.time = now;

                m_remainingdata.message += data;
                return;
            }

            //add the data from the last time
            //if there is none, use the now timestamp
            CMessage message = m_remainingdata;
            if (string.IsNullOrEmpty(message.message))
                message.time = now;

            while (nlpos != -1)
            {
                message.message += data.Substring(0, nlpos); //get the string until the newline
                //TODO: Not sure about the clone logic... need to test on messages other than "hello"
                m_messages.Add((CMessage) message.Clone());            //put the message in the queue

                //reset the message
                message.message = string.Empty;
                message.time = now;

                if (nlpos + 1 >= data.Length) //if the newline is at the end of the string, we're done here
                {
                    data = string.Empty;
                    break;
                }

                data = data.Substring(nlpos + 1); //remove all the data up to and including the newline
                nlpos = data.IndexOf('\n'); //search for a new newline
            }

            //save the remaining data with the timestamp
            m_remainingdata.message = data;
            m_remainingdata.time = now;
        }

        internal int GetRemainingDataSize()
        {
            return m_remainingdata.message.Length;
        }

        internal int GetNrMessages()
        {
            return m_messages.Count;
        }

        internal CMessage GetMessage()
        {
            CMessage message = null;

            if (m_messages.Count == 0)
                return message;

            message = m_messages[0];
            m_messages.RemoveAt(0);

            return message;
        }
    }
}
