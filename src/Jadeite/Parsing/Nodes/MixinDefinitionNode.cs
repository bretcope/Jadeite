using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.MixinDefinition)]
    public sealed class MixinDefinitionNode : INode
    {
        [AssertKind(JadeiteKind.MixinKeyword)]
        public Token MixinKeyword { get; internal set; }
        [AssertKind(JadeiteKind.HtmlIdentifier)]
        public Token Name { get; internal set; }
        [AssertKind(true, JadeiteKind.MixinParametersDefinition)]
        public BracketedNode ParametersDefinition { get; internal set; }
        [AssertKind(JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.MixinDefinition;

        internal MixinDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return MixinKeyword;
            yield return Name;
            if (ParametersDefinition != null)
                yield return ParametersDefinition;
            yield return Block;
        }
    }
}