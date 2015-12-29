using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public class ElementList : IEnumerable<ISyntaxElement>
    {
        private readonly List<ISyntaxElement> _elements = new List<ISyntaxElement>();

        public int Count => _elements.Count;
        public ISyntaxElement this[int index] => _elements[index];
        
        internal ElementList() { }

        public IEnumerator<ISyntaxElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal virtual void Add(ISyntaxElement e)
        {
            _elements.Add(e);
        }
    }

    public sealed class DocumentBodyNode : ElementList, INode
    {
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DocumentBody;

        public ElementList Children => this;
    }

    public sealed class MixinListNode : ElementList, INode
    {
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.MixinList;
        public ElementList Children => this;

        internal override void Add(ISyntaxElement e)
        {
            Debug.Assert(e.Kind == JadeiteSyntaxKind.MixinDefinition);
            base.Add(e);
        }
    }

    public sealed class TextBodyElementListNode : ElementList, INode
    {
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TextBodyElementList;
        public ElementList Children => this;

        internal override void Add(ISyntaxElement e)
        {
#if DEBUG
            switch (e.Kind)
            {
                case JadeiteSyntaxKind.HtmlText:
                
            }
#endif

            base.Add(e);
        }
    }
}