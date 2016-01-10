using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.ClassOrId)]
    public sealed class ClassOrIdNode : INode
    {
        [AssertKind(JadeiteKind.Dot, JadeiteKind.Hash)]
        public Token Prefix { get; internal set; }
        [AssertKind(JadeiteKind.HtmlIdentifier)]
        public Token Name { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.ClassOrId;
        public bool IsClass => Prefix.Kind == JadeiteKind.Dot;
        public bool IsId => Prefix.Kind == JadeiteKind.Hash;

        internal ClassOrIdNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Prefix;
            yield return Name;
        }
    }
}