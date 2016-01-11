using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(
        JadeiteKind.MemberAccess,
        JadeiteKind.Assignment,
        JadeiteKind.NullCoalescingExpression,
        JadeiteKind.ConditionalOrExpression,
        JadeiteKind.ConditionalAndExpression,
        JadeiteKind.OrExpression,
        JadeiteKind.XorExpression,
        JadeiteKind.AndExpression,
        JadeiteKind.EqualityExpression,
        JadeiteKind.RelationalExpression,
        JadeiteKind.ShiftExpression,
        JadeiteKind.AdditiveExpression,
        JadeiteKind.MultiplicativeExpression
    )]
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
                case JadeiteKind.Assignment:
                    return kind.IsAssignmentOperator();
                case JadeiteKind.NullCoalescingExpression:
                    return kind == JadeiteKind.QuestionMarkQuestionMark;
                case JadeiteKind.ConditionalOrExpression:
                    return kind == JadeiteKind.PipePipe;
                case JadeiteKind.ConditionalAndExpression:
                    return kind == JadeiteKind.AndAnd;
                case JadeiteKind.OrExpression:
                    return kind == JadeiteKind.Pipe;
                case JadeiteKind.XorExpression:
                    return kind == JadeiteKind.Caret;
                case JadeiteKind.AndExpression:
                    return kind == JadeiteKind.And;
                case JadeiteKind.EqualityExpression:
                    return kind.IsOneOf(JadeiteKind.EqualsEquals, JadeiteKind.BangEquals);
                case JadeiteKind.RelationalExpression:
                    return kind.IsOneOf(JadeiteKind.GreaterThan, JadeiteKind.LessThan, JadeiteKind.GreaterThanEquals, JadeiteKind.LessThanEquals);
                case JadeiteKind.ShiftExpression:
                    return kind.IsOneOf(JadeiteKind.LessThanLessThan, JadeiteKind.GreaterThanGreaterThan);
                case JadeiteKind.AdditiveExpression:
                    return kind.IsOneOf(JadeiteKind.Plus, JadeiteKind.Minus);
                case JadeiteKind.MultiplicativeExpression:
                    return kind.IsOneOf(JadeiteKind.Asterisk, JadeiteKind.ForwardSlash, JadeiteKind.Percent);
                default:
                    return false;
            }
        }
    }
}