using System;
using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public class BracketedNode : INode
    { 
        public Token Open { get; private set; }
        public ISyntaxElement Body { get; private set; }
        public Token Close { get; private set; }
        public JadeiteSyntaxKind Kind { get; }

        internal BracketedNode(JadeiteSyntaxKind kind)
        {
            ParsingDebug.Assert(IsBracketedKind(kind));
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Open;
            if (Body != null)
                yield return Body;
            yield return Close;
        }

        internal void SetOpen(Token open)
        {
            ParsingDebug.Assert(IsCorrectOpen(open.Kind));
            ParsingDebug.Assert(Open == null);
            Open = open;
        }

        internal void SetBody(ISyntaxElement body)
        {
            ParsingDebug.Assert(Body == null);
            Body = body;
        }

        internal void SetClose(Token close)
        {
            ParsingDebug.Assert(IsCorrectClose(close.Kind));
            ParsingDebug.Assert(Close == null);
            Close = close;
        }

        private static bool IsBracketedKind(JadeiteSyntaxKind kind)
        {
            switch (kind)
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

        private static bool IsCorrectOpen(JadeiteSyntaxKind kind)
        {
            switch (kind)
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

        private static bool IsCorrectClose(JadeiteSyntaxKind kind)
        {
            switch (kind)
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