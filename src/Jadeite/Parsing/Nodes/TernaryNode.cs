using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.ConditionalExpression)]
    public sealed class TernaryNode : INode
    {
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        [AssertNotNull]
        public Token LeftOperator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Middle { get; internal set; }
        [AssertNotNull]
        public Token RightOperator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement RightHandSide { get; internal set; }

        public JadeiteKind Kind { get; }

        internal TernaryNode(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LeftHandSide;
            yield return LeftOperator;
            yield return Middle;
            yield return RightOperator;
            yield return RightHandSide;
        }
    }
}