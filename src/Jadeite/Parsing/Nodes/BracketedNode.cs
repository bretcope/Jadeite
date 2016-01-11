using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(
        JadeiteKind.InterpolatedTag,
        JadeiteKind.EscapedInterpolatedExpression,
        JadeiteKind.UnescapedInterpolatedExpression,
        JadeiteKind.MixinParametersDefinition,
        JadeiteKind.ParenthesizedExpression,
        JadeiteKind.BracketedAssignmentTarget,
        JadeiteKind.TagAttributes,
        JadeiteKind.BracketedCodeIdentifierList
    )]
    public sealed class BracketedNode : INode, ICustomDebugNode
    {
        public Token Open { get; internal set; }
        public ISyntaxElement Body { get; internal set; }
        public Token Close { get; internal set; }
        public JadeiteKind Kind { get; }

        internal BracketedNode(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Open;
            if (Body != null)
                yield return Body;
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
                case JadeiteKind.InterpolatedTag:
                    return kind == JadeiteKind.OpenTagInterpolation;
                case JadeiteKind.EscapedInterpolatedExpression:
                    return kind == JadeiteKind.OpenEscapedInterpolation;
                case JadeiteKind.UnescapedInterpolatedExpression:
                    return kind == JadeiteKind.OpenUnscapedInterpolation;
                case JadeiteKind.MixinParametersDefinition:
                case JadeiteKind.ParenthesizedExpression:
                case JadeiteKind.BracketedAssignmentTarget:
                case JadeiteKind.TagAttributes:
                    return kind == JadeiteKind.OpenParen;
                case JadeiteKind.BracketedCodeIdentifierList:
                    return kind == JadeiteKind.OpenSquareBracket;
                default:
                    return false;
            }
        }

        private bool IsCorrectClose(JadeiteKind kind)
        {
            switch (Kind)
            {
                case JadeiteKind.InterpolatedTag:
                    return kind == JadeiteKind.CloseSquareBracket;
                case JadeiteKind.EscapedInterpolatedExpression:
                case JadeiteKind.UnescapedInterpolatedExpression:
                    return kind == JadeiteKind.CloseCurly;
                case JadeiteKind.MixinParametersDefinition:
                case JadeiteKind.ParenthesizedExpression:
                case JadeiteKind.BracketedAssignmentTarget:
                case JadeiteKind.TagAttributes:
                    return kind == JadeiteKind.CloseParen;
                case JadeiteKind.BracketedCodeIdentifierList:
                    return kind == JadeiteKind.CloseSquareBracket;
                default:
                    return false;
            }
        }
    }
}