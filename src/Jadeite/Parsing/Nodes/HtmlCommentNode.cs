using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.HtmlComment)]
    public sealed class HtmlCommentNode : INode
    {
        [AssertKind(JadeiteKind.BufferedHtmlComment, JadeiteKind.UnbufferedHtmlComment)]
        public Token Comment { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.HtmlComment;

        internal HtmlCommentNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Comment;
            yield return EndOfLine;
        }
    }
}