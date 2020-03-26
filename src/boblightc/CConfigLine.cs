namespace boblightc
{
    internal class CConfigLine
    {
        public string line { get; private set; }
        public int linenr { get; private set; }

        public CConfigLine(string buff, int linenr)
        {
            this.line = buff;
            this.linenr = linenr;
        }
    }
}
