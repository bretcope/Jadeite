using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.TextBody)]
    public sealed class TextBodyNode : INode
    {
        [AssertKind(true, JadeiteKind.TextBodyElementList)]
        public TextBodyElementListNode TextBodyElementList { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(true, JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.TextBody;

        internal TextBodyNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (TextBodyElementList != null)
                yield return TextBodyElementList;

            yield return EndOfLine;

            if (Block != null)
                yield return Block;
        }
    }
}