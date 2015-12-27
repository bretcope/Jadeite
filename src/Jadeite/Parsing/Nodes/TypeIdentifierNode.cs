using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public class TypeIdentifierNode : Node
    {
        public List<Token> Parts { get; } = new List<Token>();

        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TypeIdentifier;

        internal TypeIdentifierNode() { }

        internal void AddDot(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.Dot);

            AddChild(tok);
        }

        internal void AddIdentifier(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.CodeIdentifier || SyntaxInfo.IsOfCategory(tok.Kind, SyntaxCategory.TypeKeyword));

            AddChild(tok);
            Parts.Add(tok);
        }
    }
}