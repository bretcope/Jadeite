using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TagAttributeNode : INode, ICustomDebugNode
    {
        [AssertKind(JadeiteSyntaxKind.HtmlIdentifier)]
        public Token LeftHandSide { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.Equals, JadeiteSyntaxKind.BangEquals)]
        public Token Operator { get; internal set; }
        public ISyntaxElement RightHandSide { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TagAttribute;

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