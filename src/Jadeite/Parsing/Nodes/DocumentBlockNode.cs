using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DocumentBlockNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.Indent)]
        public Token Indent { get; internal set; }
        [AssertNotNull]
        public DocumentBodyNode Body { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.Outdent)]
        public Token Outdent { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DocumentBlock;

        internal DocumentBlockNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Indent;
            yield return Body;
            yield return Outdent;
        }
    }
}