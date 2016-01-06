using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class AndAttributesNode : INode
    {
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.AndAttributes;

        internal AndAttributesNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return null;
        }
    }
}