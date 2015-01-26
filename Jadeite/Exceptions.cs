using System;

namespace Jadeite
{
    public class JadeiteParserException : Exception
    {
        public int LineNumber { get; }

        public JadeiteParserException(int lineNumber, string message, Exception innerException = null) : base(message, innerException)
        {
            LineNumber = lineNumber;
        }

        public JadeiteParserException(string message, Exception innerException = null) : base(message, innerException)
        {
            LineNumber = -1;
        }
    }
}