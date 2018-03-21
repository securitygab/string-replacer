using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benji7425.FileStringManipulator.Replacer
{
    /// <summary>
    /// Details about an identifier discovered within a line
    /// </summary>
    public class FoundIdentifier
    {
        public string Identifier { get; private set; }
        public int IndexOf { get; set; }
        public string LineFound { get; private set; }
        public int LineFoundIdx { get; private set; }

        public FoundIdentifier(string identifier, int index, string line, int lineIdx)
        {
            this.Identifier = identifier;
            this.IndexOf = index;
            this.LineFound = line;
            this.LineFoundIdx = lineIdx;
        }
    }
}
