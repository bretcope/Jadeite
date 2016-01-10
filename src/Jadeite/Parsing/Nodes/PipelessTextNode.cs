using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.PipelessText)]
    public sealed class PipelessTextNode : INode
    {
        [AssertKind(JadeiteKind.Dot)]
        public Token Dot { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(true, JadeiteKind.PipelessTextBlock)]
        public BlockNode Body { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.PipelessText;

        internal PipelessTextNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Dot;
            yield return EndOfLine;

            if (Body != null)
                yield return Body;
        }
    }
}