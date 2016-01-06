using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class BufferedCodeNode : INode
    {
        public Token BeginningToken { get; private set; }
        public ISyntaxElement Expression { get; private set; }
        public Token EndOfLine { get; private set; }
        public JadeiteSyntaxKind Kind { get; }

        internal BufferedCodeNode(JadeiteSyntaxKind kind)
        {
            ParsingDebug.AssertKindIsOneOf(kind, JadeiteSyntaxKind.EscapedBufferedCode, JadeiteSyntaxKind.UnescapedBufferedCode);
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return BeginningToken;
            yield return Expression;
            yield return EndOfLine;
        }

        internal void SetBeginningToken(Token tok)
        {
            ParsingDebug.Assert((Kind == JadeiteSyntaxKind.EscapedBufferedCode && tok.Kind == JadeiteSyntaxKind.Equals)
                || (Kind == JadeiteSyntaxKind.UnescapedBufferedCode && tok.Kind == JadeiteSyntaxKind.BangEquals));
            ParsingDebug.Assert(BeginningToken == null);

            BeginningToken = tok;
        }

        internal void SetExpression(ISyntaxElement expression)
        {
            ParsingDebug.Assert(Expression == null);
            Expression = expression;
        }

        internal void SetEndOfLine(Token eol)
        {
            ParsingDebug.AssertKindIsOneOf(eol.Kind, JadeiteSyntaxKind.EndOfLine);
            ParsingDebug.Assert(EndOfLine == null);
            EndOfLine = eol;
        }
    }
}