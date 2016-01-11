using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.CastExpression)]
    public sealed class CastExpressionNode : INode, ICustomDebugNode
    {
        [AssertKind(JadeiteKind.OpenParen)]
        public Token Open { get; internal set; }
        [AssertNotNull]
        public Token Type { get; internal set; }
        [AssertKind(JadeiteKind.CloseParen)]
        public Token Close { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Expression { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.CastExpression;

        internal CastExpressionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Open;
            yield return Type;
            yield return Close;
            yield return Expression;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert(Kind.IsNumericTypeKeyword());
        }
    }
}