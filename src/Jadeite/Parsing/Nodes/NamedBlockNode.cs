using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public class NamedBlockNode : INode
    {
        public Token BlockKeyword { get; private set; }
        public Token AppendKeyword { get; private set; }
        public Token PrependKeyword { get; private set; }
        public Token Name { get; private set; }
        public Token EndOfLine { get; private set; }
        public DocumentBlockNode Body { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.NamedBlock;
        public bool IsAppend => AppendKeyword != null;
        public bool IsPrepend => PrependKeyword != null;

        internal NamedBlockNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (BlockKeyword != null)
                yield return BlockKeyword;
            if (AppendKeyword != null)
                yield return AppendKeyword;
            if (PrependKeyword != null)
                yield return PrependKeyword;

            yield return Name;
            yield return EndOfLine;

            if (Body != null)
                yield return Body;
        }

        internal void AddPrefix(Token tok)
        {
            if (tok.Kind == JadeiteSyntaxKind.BlockKeyword)
            {
                ParsingDebug.Assert(BlockKeyword == null);
                BlockKeyword = tok;
            }
            else if (tok.Kind == JadeiteSyntaxKind.AppendKeyword)
            {
                ParsingDebug.Assert(AppendKeyword == null);
                AppendKeyword = tok;
            }
            else
            {
                ParsingDebug.AssertKindIsOneOf(tok.Kind, JadeiteSyntaxKind.PrependKeyword);
                ParsingDebug.Assert(PrependKeyword == null);
                PrependKeyword = tok;
            }
        }

        internal void SetName(Token name)
        {
            ParsingDebug.AssertKindIsOneOf(name.Kind, JadeiteSyntaxKind.HtmlIdentifier);
            ParsingDebug.Assert(Name == null);
            Name = name;
        }

        internal void SetEndOfLine(Token eol)
        {
            ParsingDebug.AssertKindIsOneOf(eol.Kind, JadeiteSyntaxKind.EndOfLine);
            ParsingDebug.Assert(EndOfLine == null);
            EndOfLine = eol;
        }

        internal void SetBody(DocumentBlockNode body)
        {
            ParsingDebug.Assert(Body == null);
            Body = body;
        }
    }
}