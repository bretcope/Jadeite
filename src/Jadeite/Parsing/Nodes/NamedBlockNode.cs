namespace Jadeite.Parsing.Nodes
{
    public class NamedBlockNode : INode
    {
        public ElementList Children { get; } = new ElementList();
        public bool IsAppend { get; private set; }
        public bool IsPrepend { get; private set; }
        public Token Name { get; private set; }
        public DocumentBlockNode Body { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.NamedBlock;

        internal NamedBlockNode() { }

        internal void AddPrefix(Token tok)
        {
            ParsingDebug.AssertKindIsOneOf(tok.Kind, JadeiteSyntaxKind.AppendKeyword, JadeiteSyntaxKind.PrependKeyword, JadeiteSyntaxKind.BlockKeyword);

            if (tok.Kind == JadeiteSyntaxKind.AppendKeyword)
                IsAppend = true;
            else if (tok.Kind == JadeiteSyntaxKind.PrependKeyword)
                IsPrepend = true;

            Children.Add(tok);
        }

        internal void SetName(Token tok)
        {
            ParsingDebug.AssertKindIsOneOf(tok.Kind, JadeiteSyntaxKind.HtmlIdentifier);
            ParsingDebug.Assert(Name == null);

            Children.Add(tok);
            Name = tok;
        }

        internal void SetEndOfLine(Token tok)
        {
            ParsingDebug.AssertKindIsOneOf(tok.Kind, JadeiteSyntaxKind.EndOfLine);
            ParsingDebug.Assert(Name != null);

            Children.Add(tok);
        }

        internal void SetBody(DocumentBlockNode body)
        {
            ParsingDebug.Assert(Name != null);
            ParsingDebug.Assert(Body == null);

            Children.Add(body);
            Body = body;
        }
    }
}