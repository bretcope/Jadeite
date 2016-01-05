namespace Jadeite.Parsing.Nodes
{
    public sealed class HtmlCommentNode : INode
    {
        public ElementList Children { get; } = new DocumentBodyNode();
        public JadeiteSyntaxKind Kind { get; }

        internal HtmlCommentNode(JadeiteSyntaxKind kind)
        {
            ParsingDebug.AssertKindIsOneOf(kind, JadeiteSyntaxKind.BufferedHtmlComment, JadeiteSyntaxKind.UnbufferedHtmlComment);
            Kind = kind;
        }
    }
}