using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DoctypeDefinitionNode : INode
    {
        public ElementList Children { get; } = new ElementList();
        public TextBodyElementListNode Body { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DoctypeDefinition;

        internal DoctypeDefinitionNode() { }

        internal void SetDoctypeKeyword(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.DoctypeKeyword);
            Debug.Assert(Body == null);
            Children.Add(tok);
        }

        internal void SetTextBody(TextBodyElementListNode body)
        {
            Debug.Assert(Body == null);

            Children.Add(body);
            Body = body;
        }

        internal void SetEndOfLine(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.EndOfLine);
            Debug.Assert(Body != null);

            Children.Add(tok);
        }
    }
}