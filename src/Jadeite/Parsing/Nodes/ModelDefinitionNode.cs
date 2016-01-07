using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class ModelDefinitionNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.ModelKeyword)]
        public Token ModelKeyword { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.TypeIdentifier)]
        public TypeIdentifierNode TypeIdentifier { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ModelDefinition;

        internal ModelDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ModelKeyword;
            yield return TypeIdentifier;
            yield return EndOfLine;
        }
    }
}