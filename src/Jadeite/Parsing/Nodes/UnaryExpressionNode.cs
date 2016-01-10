using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(
        JadeiteKind.TagExpansion,
        JadeiteKind.InterpolatedTagExpansion,
        JadeiteKind.InterpolatedEscapedBufferedCode,
        JadeiteKind.InterpolatedUnescapedBufferedCode
    )]
    public class UnaryExpressionNode : INode
    {
        [AssertNotNull]
        public Token Operator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement RightHandSide { get; internal set; }

        public JadeiteKind Kind { get; }

        internal UnaryExpressionNode(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Operator;
            yield return RightHandSide;
        }
    }
}