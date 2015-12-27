using System.Diagnostics;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public class ModelDefinitionNode : Node
    {
        public TypeIdentifierNode TypeIdentifier { get; private set; }

        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ModelDefinition;

        internal ModelDefinitionNode() { }

        internal void SetModelKeyword(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.ModelKeyword);
            AddChild(tok);
        }

        internal void SetTypeIdentifier(TypeIdentifierNode node)
        {
            Debug.Assert(TypeIdentifier == null);

            AddChild(node);
            TypeIdentifier = node;
        }

        internal void SetEndOfLine(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.EndOfLine);
            AddChild(tok);
        }
    }
}