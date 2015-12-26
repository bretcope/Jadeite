using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public abstract class Node : ISyntaxElement
    {
        private readonly List<ISyntaxElement> _children = new List<ISyntaxElement>();

        public IReadOnlyList<ISyntaxElement> Children => _children.AsReadOnly();

        public abstract JadeiteSyntaxKind Kind { get; }
        public bool IsToken => false;
        public bool IsNode => true;
        public virtual bool IsHtmlNode => false;
        public virtual bool IsCodeNode => false;

        protected void AddChild(ISyntaxElement child)
        {
            _children.Add(child);
        }
    }

    public abstract class HtmlNode : Node
    {
        public override bool IsHtmlNode => true;
    }

    public abstract class CodeNode : Node
    {
        public override bool IsCodeNode => true;
    }

    public interface ISyntaxElement
    {
        JadeiteSyntaxKind Kind { get; }
        bool IsToken { get; }
        bool IsNode { get; }
        bool IsHtmlNode { get; }
        bool IsCodeNode { get; }
    }
}