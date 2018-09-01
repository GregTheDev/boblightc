namespace boblightc
{
    internal class CConfigLine
    {
        private string buff;
        private int linenr;

        public CConfigLine(string buff, int linenr)
        {
            this.buff = buff;
            this.linenr = linenr;
        }
    }
}