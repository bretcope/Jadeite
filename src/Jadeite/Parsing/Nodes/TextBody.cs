using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TextBody : INode
    {
        [AssertKind(true, JadeiteSyntaxKind.TextBodyElementList)]
        public TextBodyElementListNode TextBodyElementList { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.DocumentBlock)]
        public DocumentBlockNode Block { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TextBody;

        internal TextBody() { }

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