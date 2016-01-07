using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class HtmlCommentNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.BufferedHtmlComment, JadeiteSyntaxKind.UnbufferedHtmlComment)]
        public Token Comment { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.HtmlComment;

        internal HtmlCommentNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Comment;
            yield return EndOfLine;
        }
    }
}