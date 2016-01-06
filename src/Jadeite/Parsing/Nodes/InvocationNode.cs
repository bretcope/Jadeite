using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class InvocationNode : INode
    {
        public ISyntaxElement LeftHandSide { get; private set; }
        public Token Open { get; private set; }
        public ArgumentListNode ArgumentList { get; private set; }
        public Token Close { get; private set; }
        public JadeiteSyntaxKind Kind { get; }

        internal InvocationNode(JadeiteSyntaxKind kind)
        {
            ParsingDebug.Assert(IsInvocationKind(kind));
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LeftHandSide;
            yield return Open;
            yield return ArgumentList;
            yield return Close;
        }

        internal void SetLeftHandSide(ISyntaxElement e)
        {
            ParsingDebug.Assert(LeftHandSide == null);
            LeftHandSide = e;
        }

        internal void SetOpen(Token open)
        {
            ParsingDebug.Assert(IsCorrectOpen(open.Kind));
            ParsingDebug.Assert(Open == null);
            Open = open;
        }

        internal void SetArgumentList(ArgumentListNode node)
        {
            ParsingDebug.Assert(ArgumentList == null);
            ArgumentList = node;
        }

        internal void SetClose(Token close)
        {
            ParsingDebug.Assert(IsCorrectClose(close.Kind));
            ParsingDebug.Assert(Close == null);
            Close = close;
        }

        private static bool IsInvocationKind(JadeiteSyntaxKind kind)
        {
            switch (kind)
            {
                case JadeiteSyntaxKind.ExtendsDefinition:
                case JadeiteSyntaxKind.IncludeDefinition:
                case JadeiteSyntaxKind.ElementAccess:
                case JadeiteSyntaxKind.InvocationExpression:
                case JadeiteSyntaxKind.AndAttributes:
                    return true;
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
                case JadeiteSyntaxKind.AndAttributes:
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
                case JadeiteSyntaxKind.AndAttributes:
                    return kind == JadeiteSyntaxKind.CloseParen;
                default:
                    return false;
            }
        }
    }
}