using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.AndAttributes)]
    public sealed class AndAttributesNode : INode
    {
        [AssertKind(JadeiteKind.And)]
        public Token And { get; internal set; }
        [AssertKind(JadeiteKind.AttributesKeyword)]
        public Token AttributesKeyword { get; internal set; }
        [AssertKind(JadeiteKind.OpenParen)]
        public Token OpenParen { get; internal set; }
        [AssertKind(true, JadeiteKind.ArgumentList)]
        public ArgumentListNode Arguments { get; internal set; }
        [AssertKind(JadeiteKind.CloseParen)]
        public Token CloseParen { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.AndAttributes;

        internal AndAttributesNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return And;
            yield return AttributesKeyword;
            yield return OpenParen;

            if (Arguments != null)
                yield return Arguments;

            yield return CloseParen;
        }
    }
}