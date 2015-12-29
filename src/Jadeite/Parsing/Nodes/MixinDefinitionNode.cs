namespace Jadeite.Parsing.Nodes
{
    public sealed class MixinDefinitionNode : INode
    {
        public ElementList Children { get; } = new ElementList();

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.MixinDefinition;

        internal MixinDefinitionNode() { }
    }
}