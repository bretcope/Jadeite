namespace Jadeite.Parsing.Nodes
{
    public sealed class BufferedCodeNode : INode
    {
        public ElementList Children { get; } = new ElementList();

        public JadeiteSyntaxKind Kind { get; }

        internal BufferedCodeNode(JadeiteSyntaxKind kind)
        {
            ParsingDebug.AssertKindIsOneOf(kind, JadeiteSyntaxKind.EscapedBufferedCode, JadeiteSyntaxKind.UnescapedBufferedCode);
            Kind = kind;
        }

        //
    }
}