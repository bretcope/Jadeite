using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public abstract class Node : ISyntaxElement
    {
        private readonly List<ISyntaxElement> _children = new List<ISyntaxElement>();

        public IReadOnlyList<ISyntaxElement> Children => _children.AsReadOnly();

        public abstract JadeiteSyntaxKind Kind { get; }

        protected void AddChild(ISyntaxElement child)
        {
            _children.Add(child);
        }
    }
}