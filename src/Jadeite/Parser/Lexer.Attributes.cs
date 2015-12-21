using System;

namespace Jadeite.Parser
{
    public partial class Lexer
    {
        private void TransitionToAttributes()
        {
            PushState(LexerState.Attributes);
        }

        private void ScanAttributes()
        {
            ConsumeWhiteSpaceAsTrivia();

            switch (CurrentChar())
            {
                case ')':
                    ConsumeToken(JadeiteSyntaxKind.CloseParen, 1);
                    ExitState();
                    return;
                case '=':
                    ConsumeToken(JadeiteSyntaxKind.Equals, 1);
                    TransitionToCode(CodeScanMode.Attributes);
                    return;
                case '!':
                    if (NextChar() != '=')
                        throw new Exception($"Expected token at Line {Line} Column {Column}."); // todo
                    ConsumeToken(JadeiteSyntaxKind.BangEquals, 2);
                    TransitionToCode(CodeScanMode.Attributes);
                    return;
                case ',':
                    ConsumeToken(JadeiteSyntaxKind.Comma, 1);
                    return;
                case '\r':
                case '\n':
                case INVALID_CHAR:
                    ExitState();
                    return;
                default:
                    ScanHtmlIdentifierOrThrow();
                    return;
            }
        }
    }
}
