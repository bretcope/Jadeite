using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public class Token : ISyntaxElement
    {
        public JadeiteKind Kind { get; internal set; }
        public string Text { get; internal set; }
        public object Value { get; internal set; }
        public Position Position { get; internal set; }
        public string LeadingTrivia { get; internal set; }
        public string TrailingTrivia { get; internal set; }

        internal Token()
        {
        }
    }
}