namespace Jadeite.Parsing.Nodes
{
    public sealed class DocumentBlockNode : INode
    {
        public ElementList Children { get; } = new DocumentBodyNode();
        
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DocumentBlock;

        internal DocumentBlockNode() { }
    }
}