using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class MixinDefinitionNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.MixinKeyword)]
        public Token MixinKeyword { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.HtmlIdentifier)]
        public Token Name { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.MixinParametersDefinition)]
        public BracketedNode ParametersDefinition { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.DocumentBlock)]
        public DocumentBlockNode Body { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.MixinDefinition;

        internal MixinDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return MixinKeyword;
            yield return Name;
            if (ParametersDefinition != null)
                yield return ParametersDefinition;
            yield return Body;
        }
    }
}