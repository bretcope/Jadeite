using System;

namespace Jadeite.Parsing
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
                    ConsumeToken(JadeiteKind.CloseParen, 1);
                    ExitState();
                    return;
                case '=':
                    ConsumeToken(JadeiteKind.Equals, 1);
                    TransitionToCode(CodeScanMode.Attributes);
                    return;
                case '!':
                    if (NextChar() != '=')
                        throw new Exception($"Expected token at Line {Line} Column {Column}."); // todo
                    ConsumeToken(JadeiteKind.BangEquals, 2);
                    TransitionToCode(CodeScanMode.Attributes);
                    return;
                case ',':
                    ConsumeToken(JadeiteKind.Comma, 1);
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
