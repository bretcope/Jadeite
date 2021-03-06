﻿
namespace Jadeite.Parsing
{
    public partial class Lexer
    {
        private void TransitionToNamedBlock()
        {
            PushState(LexerState.NamedBlock);
        }

        private void ScanNamedBlock()
        {
            ConsumeWhiteSpaceAsTrivia();

            switch (CurrentChar())
            {
                case '\r':
                case '\n':
                case INVALID_CHAR:
                    ExitState();
                    return;
                case 'a':
                    if (TryConsumeKeyword(JadeiteKind.AppendKeyword))
                        return;

                    goto default;
                case 'p':
                    if (TryConsumeKeyword(JadeiteKind.PrependKeyword))
                        return;

                    goto default;
                default:
                    ScanHtmlIdentifierOrThrow();
                    return;
            }
        }
    }
}
