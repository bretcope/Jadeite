using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TypeIdentifierNode : INode
    {
        public ElementList Children { get; } = new ElementList();
        public List<Token> Parts { get; } = new List<Token>();

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TypeIdentifier;

        internal TypeIdentifierNode() { }

        internal void AddDot(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.Dot);

            Children.Add(tok);
        }

        internal void AddIdentifier(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.CodeIdentifier || SyntaxInfo.IsOfCategory(tok.Kind, SyntaxCategory.TypeKeyword));

            Children.Add(tok);
            Parts.Add(tok);
        }
    }
}