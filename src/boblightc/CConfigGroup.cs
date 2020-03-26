using System.Collections.Generic;

namespace boblightc
{
    internal class CConfigGroup
    {
        public List<CConfigLine> lines { get; private set; }

        public CConfigGroup()
        {
            lines = new List<CConfigLine>();
        }
    }
}
