using System.Diagnostics;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing.Nodes
{
    public sealed class ModelDefinitionNode : INode
    {
        public ElementList Children { get; } = new ElementList();
        public TypeIdentifierNode TypeIdentifier { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ModelDefinition;

        internal ModelDefinitionNode() { }

        internal void SetModelKeyword(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.ModelKeyword);
            Children.Add(tok);
        }

        internal void SetTypeIdentifier(TypeIdentifierNode node)
        {
            Debug.Assert(TypeIdentifier == null);

            Children.Add(node);
            TypeIdentifier = node;
        }

        internal void SetEndOfLine(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.EndOfLine);
            Children.Add(tok);
        }
    }
}