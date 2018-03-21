using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benji7425.FileStringManipulator.Replacer
{
    [Serializable]
    public class IdentifierNotFoundException : Exception
    {
        public string File { get; private set; }
        public string Line { get; private set; }
        public string Identifier { get; private set; }

        public IdentifierNotFoundException(string file, string line, string identifier, string message) : this(file, line, identifier, message, null)
        {
        }

        public IdentifierNotFoundException(string file, string line, string identifier, string message, Exception inner) : base(message, inner)
        {
            this.File = file;
            this.Line = line;
            this.Identifier = identifier;
        }
    }
}
