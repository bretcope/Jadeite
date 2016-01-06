using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class HtmlCommentNode : INode
    {
        public Token Comment { get; private set; }
        public Token EndOfLine { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.HtmlComment;

        internal HtmlCommentNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Comment;
            yield return EndOfLine;
        }

        internal void SetComment(Token comment)
        {
            ParsingDebug.AssertKindIsOneOf(comment.Kind, JadeiteSyntaxKind.BufferedHtmlComment, JadeiteSyntaxKind.UnbufferedHtmlComment);
            ParsingDebug.Assert(Comment == null);
            Comment = comment;
        }

        internal void SetEndOfLine(Token eol)
        {
            ParsingDebug.AssertKindIsOneOf(eol.Kind, JadeiteSyntaxKind.EndOfLine);
            ParsingDebug.Assert(EndOfLine == null);
            EndOfLine = eol;
        }
    }
}