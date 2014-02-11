using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM.Initiator.Classes
{
    [Serializable]
    public class ParseException : ApplicationException
    {
        public string ParseText { get; private set; }

        public ParseException(string message)
            : this(message, "", null)
        {
        }

        public ParseException(string message, string parseText)
            : this(message, parseText, null)
        {
            ParseText = parseText;
        }

        public ParseException(string message, string parseText, Exception innerException)
            : base(message, innerException)
        {
            ParseText = parseText;
        }
    }
}
