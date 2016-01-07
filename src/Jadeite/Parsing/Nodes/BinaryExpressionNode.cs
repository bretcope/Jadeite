using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class BinaryExpressionNode : INode, ICustomDebugNode
    {
        public JadeiteSyntaxKind Kind { get; }
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        public Token Operator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement RightHandSide { get; internal set; }

        internal BinaryExpressionNode(JadeiteSyntaxKind kind)
        {
            ParsingDebug.Assert(IsBinaryExpressionKind(kind));
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LeftHandSide;
            yield return Operator;
            yield return RightHandSide;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert(IsValidOperator(Operator.Kind));
        }

        private static bool IsBinaryExpressionKind(JadeiteSyntaxKind kind)
        {
            switch (kind)
            {
                case JadeiteSyntaxKind.MemberAccess:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsValidOperator(JadeiteSyntaxKind kind)
        {
            switch (Kind)
            {
                case JadeiteSyntaxKind.MemberAccess:
                    return kind == JadeiteSyntaxKind.Dot;
                default:
                    return false;
            }
        }
    }
}