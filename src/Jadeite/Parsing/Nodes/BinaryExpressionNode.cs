using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.MemberAccess)]
    public sealed class BinaryExpressionNode : INode, ICustomDebugNode
    {
        public JadeiteKind Kind { get; }
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        public Token Operator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement RightHandSide { get; internal set; }

        internal BinaryExpressionNode(JadeiteKind kind)
        {
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

        private bool IsValidOperator(JadeiteKind kind)
        {
            switch (Kind)
            {
                case JadeiteKind.MemberAccess:
                    return kind == JadeiteKind.Dot;
                default:
                    return false;
            }
        }
    }
}