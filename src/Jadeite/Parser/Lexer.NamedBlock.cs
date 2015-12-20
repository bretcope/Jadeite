
namespace Jadeite.Parser
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
                    if (TryConsumeKeyword(TokenType.Append))
                        return;

                    goto default;
                case 'p':
                    if (TryConsumeKeyword(TokenType.Prepend))
                        return;

                    goto default;
                default:
                    ScanHtmlIdentifierOrThrow();
                    return;
            }
        }
    }
}
