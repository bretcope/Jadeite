using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class AndAttributesNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.And)]
        public Token And { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.AttributesKeyword)]
        public Token AttributesKeyword { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.OpenParen)]
        public Token OpenParen { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.ArgumentList)]
        public ArgumentListNode Arguments { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.CloseParen)]
        public Token CloseParen { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.AndAttributes;

        internal AndAttributesNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return And;
            yield return AttributesKeyword;
            yield return OpenParen;

            if (Arguments != null)
                yield return Arguments;

            yield return CloseParen;
        }
    }
}