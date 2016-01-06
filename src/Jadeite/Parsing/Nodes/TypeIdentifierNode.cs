using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TypeIdentifierNode : INode
    {
        public SyntaxList<Token> Parts { get; } = new SyntaxList<Token>();

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TypeIdentifier;

        internal TypeIdentifierNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            return Parts;
        }

        internal void AddDot(Token tok)
        {
            ParsingDebug.AssertKindIsOneOf(tok.Kind, JadeiteSyntaxKind.Dot);
            Parts.Add(tok);
        }

        internal void AddIdentifier(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.CodeIdentifier || SyntaxInfo.IsOfCategory(tok.Kind, SyntaxCategory.TypeKeyword));
            Parts.Add(tok);
        }
    }
}