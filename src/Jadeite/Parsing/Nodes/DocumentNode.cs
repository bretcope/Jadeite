using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public class DocumentNode : Node, IParentElement
    {
        public InvocationNode Extends { get; private set; }
        public ElementList Body { get; private set; }

        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Document;

        internal DocumentNode() { }

        internal void SetExtendsDefinition(InvocationNode node)
        {
            Debug.Assert(node.Kind == JadeiteSyntaxKind.ExtendsDefinition);
            Debug.Assert(Extends == null);

            AddChild(node);
            Extends = node;
        }

        internal void AddEndOfLine(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.EndOfLine);
            Debug.Assert(Extends == null);
            Debug.Assert(Body == null);

            AddChild(tok);
        }
    }
}