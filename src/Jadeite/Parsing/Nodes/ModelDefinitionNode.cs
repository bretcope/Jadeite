using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.ModelDefinition)]
    public sealed class ModelDefinitionNode : INode
    {
        [AssertKind(JadeiteKind.ModelKeyword)]
        public Token ModelKeyword { get; internal set; }
        [AssertKind(JadeiteKind.TypeIdentifier)]
        public TypeIdentifierNode TypeIdentifier { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.ModelDefinition;

        internal ModelDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ModelKeyword;
            yield return TypeIdentifier;
            yield return EndOfLine;
        }
    }
}