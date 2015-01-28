
using System.Text.RegularExpressions;

namespace Jadeite.Parser.Nodes
{
    public class CodeNode : Node
    {
        public override string Type => "Code";

        public string Value { get; }
        public bool Buffer { get; }
        public bool Escape { get; }

        public CodeNode(string value, bool buffer, bool escape)
        {
            Value = value;
            Buffer = buffer;
            Escape = escape;

            if (Regex.IsMatch(value, "^ *else"))
                Debug = false;
        }
    }
}
