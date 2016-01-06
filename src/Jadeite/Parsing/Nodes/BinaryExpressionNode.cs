using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class BinaryExpressionNode : INode
    {
        public JadeiteSyntaxKind Kind { get; }
        public ISyntaxElement LeftHandSide { get; private set; }
        public Token Operator { get; private set; }
        public ISyntaxElement RightHandSide { get; private set; }

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

        internal void SetLeftHandSide(ISyntaxElement e)
        {
            ParsingDebug.Assert(LeftHandSide == null);
            LeftHandSide = e;
        }

        internal void SetOperator(Token tok)
        {
            ParsingDebug.Assert(IsBinaryExpressionOperator(tok.Kind));
            ParsingDebug.Assert(Operator == null);
            Operator = tok;
        }

        internal void SetRightHandSide(ISyntaxElement e)
        {
            ParsingDebug.Assert(RightHandSide == null);
            RightHandSide = e;
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

        private static bool IsBinaryExpressionOperator(JadeiteSyntaxKind kind)
        {
            switch (kind)
            {
                case JadeiteSyntaxKind.Dot:
                    return true;
                default:
                    return false;
            }
        }
    }
}