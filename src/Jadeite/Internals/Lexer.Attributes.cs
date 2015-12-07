
using System;
using System.Diagnostics;

namespace Jadeite.Internals
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
                    ConsumeToken(TokenType.CloseParen, 1);
                    ExitState();
                    return;
                case '=':
                    ConsumeToken(TokenType.Equals, 1);
                    TransitionToCode(CodeScanMode.Attributes);
                    return;
                case ',':
                    ConsumeToken(TokenType.Comma, 1);
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
