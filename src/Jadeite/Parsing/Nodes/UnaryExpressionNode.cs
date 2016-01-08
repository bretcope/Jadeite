using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public class UnaryExpressionNode : INode, ICustomDebugNode
    {
        [AssertNotNull]
        public Token Operator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement RightHandSide { get; internal set; }

        public JadeiteSyntaxKind Kind { get; }

        internal UnaryExpressionNode(JadeiteSyntaxKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Operator;
            yield return RightHandSide;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert(IsUnaryKind());
        }

        private bool IsUnaryKind()
        {
            switch (Kind)
            {
                case JadeiteSyntaxKind.TagExpansion:
                case JadeiteSyntaxKind.InterpolatedTagExpansion:
                case JadeiteSyntaxKind.InterpolatedEscapedBufferedCode:
                case JadeiteSyntaxKind.InterpolatedUnescapedBufferedCode:
                    return true;
                default:
                    return false;
            }
        }
    }
}