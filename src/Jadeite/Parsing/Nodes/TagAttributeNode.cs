using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TagAttributeNode : INode
    {
        public Token LeftHandSide { get; private set; }
        public Token Operator { get; private set; }
        public ISyntaxElement RightHandSide { get; private set; }

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

        internal void SetLeftHandSide(Token left)
        {
            ParsingDebug.AssertKindIsOneOf(left.Kind, JadeiteSyntaxKind.HtmlIdentifier);
            ParsingDebug.Assert(LeftHandSide == null);
            LeftHandSide = left;
        }

        internal void SetOperator(Token op)
        {
            ParsingDebug.AssertKindIsOneOf(op.Kind, JadeiteSyntaxKind.Equals, JadeiteSyntaxKind.BangEquals);
            ParsingDebug.Assert(Operator == null);
            Operator = op;
        }

        internal void SetRightHandSide(ISyntaxElement expression)
        {
            ParsingDebug.Assert(RightHandSide == null);
            RightHandSide = expression;
        }
    }
}