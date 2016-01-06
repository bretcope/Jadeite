using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class ModelDefinitionNode : INode
    {
        public Token ModelKeyword { get; private set; }
        public TypeIdentifierNode TypeIdentifier { get; private set; }
        public Token EndOfLine { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ModelDefinition;

        internal ModelDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ModelKeyword;
            yield return TypeIdentifier;
            yield return EndOfLine;
        }

        internal void SetModelKeyword(Token model)
        {
            ParsingDebug.AssertKindIsOneOf(model.Kind, JadeiteSyntaxKind.ModelKeyword);
            ParsingDebug.Assert(ModelKeyword == null);
            ModelKeyword = model;
        }

        internal void SetTypeIdentifier(TypeIdentifierNode node)
        {
            ParsingDebug.Assert(TypeIdentifier == null);
            TypeIdentifier = node;
        }

        internal void SetEndOfLine(Token endOfLine)
        {
            ParsingDebug.AssertKindIsOneOf(endOfLine.Kind, JadeiteSyntaxKind.EndOfLine);
            ParsingDebug.Assert(EndOfLine == null);
            EndOfLine = endOfLine;
        }
    }
}