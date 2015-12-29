
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class ArgumentListNode : INode
    {
        public List<ISyntaxElement> Arguments { get; } = new List<ISyntaxElement>();
        public ElementList Children { get; } = new ElementList();

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ArgumentList;

        internal ArgumentListNode() { }

        internal void AddComma(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.Comma);
            Children.Add(tok);
        }

        internal void AddArgument(ISyntaxElement e)
        {
            Children.Add(e);
            Arguments.Add(e);
        }
    }
}
