using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class ClassOrIdNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.Dot, JadeiteSyntaxKind.Hash)]
        public Token Prefix { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.HtmlIdentifier)]
        public Token Name { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ClassOrId;
        public bool IsClass => Prefix.Kind == JadeiteSyntaxKind.Dot;
        public bool IsId => Prefix.Kind == JadeiteSyntaxKind.Hash;

        internal ClassOrIdNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Prefix;
            yield return Name;
        }
    }
}