namespace Jadeite.Parsing.Nodes
{
    public sealed class UnbufferedCodeNode : INode
    {
        public ElementList Children { get; } = new DocumentBodyNode();

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.UnbufferedCode;

        internal UnbufferedCodeNode() { }
    }
}