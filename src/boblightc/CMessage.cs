using System;

namespace boblightc
{
    internal class CMessage : ICloneable
    {
        internal long time;

        public string message;

        public object Clone()
        {
            return new CMessage
            {
                time = this.time,
                message = this.message
            };
        }
    }
}
