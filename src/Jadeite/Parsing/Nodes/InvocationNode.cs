using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class InvocationNode : INode, ICustomDebugNode
    {
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        [AssertNotNull]
        public Token Open { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.ArgumentList)]
        public ArgumentListNode ArgumentList { get; internal set; }
        [AssertNotNull]
        public Token Close { get; internal set; }
        public JadeiteSyntaxKind Kind { get; }

        internal InvocationNode(JadeiteSyntaxKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LeftHandSide;
            yield return Open;
            yield return ArgumentList;
            yield return Close;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert(IsInvocationKind(Kind));
            ParsingDebug.Assert(IsCorrectOpen(Open.Kind));
            ParsingDebug.Assert(IsCorrectClose(Close.Kind));
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