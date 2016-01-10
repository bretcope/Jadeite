using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.BufferedCodeBody)]
    public sealed class BufferedCodeBodyNode : INode
    {
        [AssertKind(JadeiteKind.EscapedBufferedCode, JadeiteKind.UnescapedBufferedCode)]
        public BufferedCodeNode BufferedCode { get; internal set; }
        [AssertKind(true, JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.BufferedCodeBody;

        internal BufferedCodeBodyNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return BufferedCode;

            if (Block != null)
                yield return Block;
        }
    }
}