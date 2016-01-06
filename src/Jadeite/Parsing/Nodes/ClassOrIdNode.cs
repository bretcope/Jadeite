using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class ClassOrIdNode : INode
    {
        public Token Prefix { get; private set; }
        public Token Name { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ClassOrId;
        public bool IsClass => Prefix.Kind == JadeiteSyntaxKind.Dot;
        public bool IsId => Prefix.Kind == JadeiteSyntaxKind.Hash;

        internal ClassOrIdNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Prefix;
            yield return Name;
        }

        internal void SetPrefix(Token prefix)
        {
            ParsingDebug.AssertKindIsOneOf(prefix.Kind, JadeiteSyntaxKind.Dot, JadeiteSyntaxKind.Hash);
            ParsingDebug.Assert(Prefix == null);
            Prefix = prefix;
        }

        internal void SetName(Token name)
        {
            ParsingDebug.AssertKindIsOneOf(name.Kind, JadeiteSyntaxKind.HtmlIdentifier);
            ParsingDebug.Assert(Name == null);
            Name = name;
        }
    }
}