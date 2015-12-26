namespace Jadeite.Parsing.Nodes
{
    public class MixinDefinitionNode : Node
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.MixinDefinition;

        internal MixinDefinitionNode() { }
    }
}