using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.PostIncrementExpression, JadeiteKind.PostDecrementExpression)]
    public sealed class PostfixExpression : INode, ICustomDebugNode
    {
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        [AssertNotNull]
        public Token Operator { get; internal set; }

        public JadeiteKind Kind { get; }

        internal PostfixExpression(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LeftHandSide;
            yield return Operator;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            if (Kind == JadeiteKind.PostIncrementExpression)
                ParsingDebug.AssertKindIsOneOf(Operator.Kind, JadeiteKind.PlusPlus);
            else 
                ParsingDebug.AssertKindIsOneOf(Operator.Kind, JadeiteKind.MinusMinus);
        }
    }
}