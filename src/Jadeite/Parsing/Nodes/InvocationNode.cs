using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(
        JadeiteKind.ExtendsDefinition,
        JadeiteKind.IncludeDefinition,
        JadeiteKind.ElementAccess,
        JadeiteKind.InvocationExpression,
        JadeiteKind.AndAttributes
    )]
    public sealed class InvocationNode : INode, ICustomDebugNode
    {
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        [AssertNotNull]
        public Token Open { get; internal set; }
        [AssertKind(JadeiteKind.ArgumentList)]
        public ArgumentListNode ArgumentList { get; internal set; }
        [AssertNotNull]
        public Token Close { get; internal set; }
        public JadeiteKind Kind { get; }

        internal InvocationNode(JadeiteKind kind)
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
            ParsingDebug.Assert(IsCorrectOpen(Open.Kind));
            ParsingDebug.Assert(IsCorrectClose(Close.Kind));
        }

        private bool IsCorrectOpen(JadeiteKind kind)
        {
            switch (Kind)
            {
                case JadeiteKind.ElementAccess:
                    return kind == JadeiteKind.OpenSquareBracket;
                case JadeiteKind.ExtendsDefinition:
                case JadeiteKind.IncludeDefinition:
                case JadeiteKind.InvocationExpression:
                case JadeiteKind.AndAttributes:
                    return kind == JadeiteKind.OpenParen;
                default:
                    return false;
            }
        }

        private bool IsCorrectClose(JadeiteKind kind)
        {
            switch (Kind)
            {
                case JadeiteKind.ElementAccess:
                    return kind == JadeiteKind.CloseSquareBracket;
                case JadeiteKind.ExtendsDefinition:
                case JadeiteKind.IncludeDefinition:
                case JadeiteKind.InvocationExpression:
                case JadeiteKind.AndAttributes:
                    return kind == JadeiteKind.CloseParen;
                default:
                    return false;
            }
        }
    }
}