using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DocumentNode : INode, IParentElement
    {
        public ElementList Children { get; } = new ElementList();
        public InvocationNode Extends { get; private set; }
        public ElementList Body { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Document;

        internal DocumentNode() { }

        internal void SetExtendsDefinition(InvocationNode node)
        {
            Debug.Assert(node.Kind == JadeiteSyntaxKind.ExtendsDefinition);
            Debug.Assert(Extends == null);

            Children.Add(node);
            Extends = node;
        }

        internal void AddEndOfLine(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.EndOfLine);
            Debug.Assert(Extends == null);
            Debug.Assert(Body == null);

            Children.Add(tok);
        }

        internal void SetDocumentBody(DocumentBodyNode node)
        {
            Debug.Assert(Body == null);

            Children.Add(node);
            Body = node;
        }
    }
}