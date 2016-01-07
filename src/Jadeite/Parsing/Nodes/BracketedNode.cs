using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public class BracketedNode : INode, ICustomDebugNode
    {
        public Token Open { get; internal set; }
        public ISyntaxElement Body { get; internal set; }
        public Token Close { get; internal set; }
        public JadeiteSyntaxKind Kind { get; }

        internal BracketedNode(JadeiteSyntaxKind kind)
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
            ParsingDebug.Assert(IsBracketedKind(Kind));
            ParsingDebug.Assert(IsCorrectOpen(Open.Kind));
            ParsingDebug.Assert(IsCorrectClose(Close.Kind));
        }

        private bool IsBracketedKind(JadeiteSyntaxKind kind)
        {
            switch (Kind)
            {
                case JadeiteSyntaxKind.InterpolatedTag:
                case JadeiteSyntaxKind.EscapedInterpolatedExpression:
                case JadeiteSyntaxKind.UnescapedInterpolatedExpression:
                case JadeiteSyntaxKind.MixinParametersDefinition:
                case JadeiteSyntaxKind.ParenthesizedExpression:
                case JadeiteSyntaxKind.ParenthesizedAssignmentTarget:
                case JadeiteSyntaxKind.TagAttributes:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsCorrectOpen(JadeiteSyntaxKind kind)
        {
            switch (Kind)
            {
                case JadeiteSyntaxKind.InterpolatedTag:
                    return kind == JadeiteSyntaxKind.OpenTagInterpolation;
                case JadeiteSyntaxKind.EscapedInterpolatedExpression:
                    return kind == JadeiteSyntaxKind.OpenEscapedInterpolation;
                case JadeiteSyntaxKind.UnescapedInterpolatedExpression:
                    return kind == JadeiteSyntaxKind.OpenNonEscapedInterpolation;
                case JadeiteSyntaxKind.MixinParametersDefinition:
                case JadeiteSyntaxKind.ParenthesizedExpression:
                case JadeiteSyntaxKind.ParenthesizedAssignmentTarget:
                case JadeiteSyntaxKind.TagAttributes:
                    return kind == JadeiteSyntaxKind.OpenParen;
                default:
                    return false;
            }
        }

        private bool IsCorrectClose(JadeiteSyntaxKind kind)
        {
            switch (Kind)
            {
                case JadeiteSyntaxKind.InterpolatedTag:
                    return kind == JadeiteSyntaxKind.CloseSquareBracket;
                case JadeiteSyntaxKind.EscapedInterpolatedExpression:
                case JadeiteSyntaxKind.UnescapedInterpolatedExpression:
                    return kind == JadeiteSyntaxKind.CloseCurly;
                case JadeiteSyntaxKind.MixinParametersDefinition:
                case JadeiteSyntaxKind.ParenthesizedExpression:
                case JadeiteSyntaxKind.ParenthesizedAssignmentTarget:
                case JadeiteSyntaxKind.TagAttributes:
                    return kind == JadeiteSyntaxKind.CloseParen;
                default:
                    return false;
            }
        }
    }
}