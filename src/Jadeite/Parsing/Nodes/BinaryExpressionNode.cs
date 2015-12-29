using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class BinaryExpressionNode : INode
    {
        public JadeiteSyntaxKind Kind { get; }
        public ElementList Children { get; } = new ElementList();
        public ISyntaxElement LeftHandSide { get; private set; }
        public Token Operator { get; private set; }
        public ISyntaxElement RightHandSide { get; private set; }

        internal BinaryExpressionNode(JadeiteSyntaxKind kind)
        {
            Debug.Assert(IsBinaryExpressionKind(kind));
            Kind = kind;
        }

        internal void SetLeftHandSide(ISyntaxElement e)
        {
            Debug.Assert(LeftHandSide == null);
            Debug.Assert(Operator == null);
            Debug.Assert(RightHandSide == null);

            Children.Add(e);
            LeftHandSide = e;
        }

        internal void SetOperator(Token tok)
        {
            Debug.Assert(IsBinaryExpressionOperator(tok.Kind));
            Debug.Assert(LeftHandSide != null);
            Debug.Assert(Operator == null);
            Debug.Assert(RightHandSide == null);

            Children.Add(tok);
            Operator = tok;
        }

        internal void SetRightHandSide(ISyntaxElement e)
        {
            Debug.Assert(LeftHandSide != null);
            Debug.Assert(Operator != null);
            Debug.Assert(RightHandSide == null);

            Children.Add(e);
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