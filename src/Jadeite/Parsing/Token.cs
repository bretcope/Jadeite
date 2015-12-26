using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public class Token : ISyntaxElement
    {
        public JadeiteSyntaxKind Kind { get; internal set; }
        public string Text { get; internal set; }
        public object Value { get; internal set; }
        public Position Position { get; internal set; }
        public string LeadingTrivia { get; internal set; }
        public string TrailingTrivia { get; internal set; }

        public bool IsToken => true;
        public bool IsNode => false;
        public bool IsHtmlNode => false;
        public bool IsCodeNode => false;

        internal Token()
        {
        }
    }
}