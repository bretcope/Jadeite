using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public class InvocationNode : Node
    {
        public override JadeiteSyntaxKind Kind { get; }
        public ISyntaxElement LeftHandSide { get; private set; }
        public ArgumentListNode ArgumentList { get; private set; }

        internal InvocationNode(JadeiteSyntaxKind kind)
        {
            Debug.Assert(IsInvocationKind(kind));
            Kind = kind;
        }

        internal void SetLeftHandSide(ISyntaxElement e)
        {
            Debug.Assert(LeftHandSide == null);

            AddChild(e);
            LeftHandSide = e;
        }

        internal void SetArgumentList(ArgumentListNode node)
        {
            Debug.Assert(ArgumentList == null);

            AddChild(node);
            ArgumentList = node;
        }

        internal void SetOpen(Token tok)
        {
            Debug.Assert(IsCorrectOpen(tok.Kind));
            Debug.Assert(LeftHandSide != null);
            Debug.Assert(ArgumentList == null);

            AddChild(tok);
        }

        internal void SetClose(Token tok)
        {
            Debug.Assert(IsCorrectClose(tok.Kind));

            AddChild(tok);
        }

        private static bool IsInvocationKind(JadeiteSyntaxKind kind)
        {
            switch (kind)
            {
                case JadeiteSyntaxKind.ExtendsDefinition:
                case JadeiteSyntaxKind.IncludeDefinition:
                case JadeiteSyntaxKind.ElementAccess:
                case JadeiteSyntaxKind.InvocationExpression:
                default:
                    return false;
            }
        }

        private bool IsCorrectOpen(JadeiteSyntaxKind kind)
        {
            switch (Kind)
            {
                case JadeiteSyntaxKind.ElementAccess:
                    return kind == JadeiteSyntaxKind.OpenSquareBracket;
                case JadeiteSyntaxKind.ExtendsDefinition:
                case JadeiteSyntaxKind.IncludeDefinition:
                case JadeiteSyntaxKind.InvocationExpression:
                    return kind == JadeiteSyntaxKind.OpenParen;
                default:
                    return false;
            }
        }

        private bool IsCorrectClose(JadeiteSyntaxKind kind)
        {
            switch (Kind)
            {
                case JadeiteSyntaxKind.ElementAccess:
                    return kind == JadeiteSyntaxKind.CloseSquareBracket;
                case JadeiteSyntaxKind.ExtendsDefinition:
                case JadeiteSyntaxKind.IncludeDefinition:
                case JadeiteSyntaxKind.InvocationExpression:
                    return kind == JadeiteSyntaxKind.CloseParen;
                default:
                    return false;
            }
        }
    }
}