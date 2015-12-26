
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public class ArgumentListNode : Node
    {
        public List<ISyntaxElement> Arguments { get; } = new List<ISyntaxElement>();

        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ArgumentList;

        internal ArgumentListNode() { }

        internal void AddComma(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.Comma);
            AddChild(tok);
        }

        internal void AddArgument(ISyntaxElement e)
        {
            AddChild(e);
            Arguments.Add(e);
        }
    }
}
