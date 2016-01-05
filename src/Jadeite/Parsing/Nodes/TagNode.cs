namespace Jadeite.Parsing.Nodes
{
    public sealed class TagNode : INode
    {
        public ElementList Children { get; } = new DocumentBodyNode();

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Tag;

        internal TagNode() { }
    }
}