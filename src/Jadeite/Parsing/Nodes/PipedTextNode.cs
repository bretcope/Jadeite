using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.PipedText)]
    public sealed class PipedTextNode : INode
    {
        [AssertKind(JadeiteKind.Pipe)]
        public Token Pipe { get; internal set; }
        [AssertKind(JadeiteKind.TextBodyElementList)]
        public TextBodyElementListNode Body { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.PipedText;

        internal PipedTextNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Pipe;
            yield return Body;
            yield return EndOfLine;
        }
    }
}