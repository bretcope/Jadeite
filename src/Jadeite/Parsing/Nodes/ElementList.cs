using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public class ElementList : List<ISyntaxElement>, ISyntaxElement
    {
        public JadeiteSyntaxKind Kind { get; }

        internal ElementList(JadeiteSyntaxKind kind)
        {
            Kind = kind;
        } 
    }
}