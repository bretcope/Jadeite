using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.TagAttribute)]
    public sealed class TagAttributeNode : INode, ICustomDebugNode
    {
        [AssertKind(JadeiteKind.HtmlIdentifier)]
        public Token LeftHandSide { get; internal set; }
        [AssertKind(true, JadeiteKind.Equals, JadeiteKind.BangEquals)]
        public Token Operator { get; internal set; }
        public ISyntaxElement RightHandSide { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.TagAttribute;

        internal TagAttributeNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LeftHandSide;
            if (Operator != null)
            {
                yield return Operator;
                yield return RightHandSide;
            }
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert((Operator == null) == (RightHandSide == null));
        }
    }
}