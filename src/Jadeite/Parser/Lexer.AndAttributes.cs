
namespace Jadeite.Parser
{
    public partial class Lexer
    {
        private void TransitionToAndAttributes()
        {
            PushState(LexerState.AndAttributes);
        }

        private void ScanAndAttributes()
        {
            ConsumeWhiteSpaceAsTrivia();

            switch (CurrentChar())
            {
                case ')':
                    ConsumeToken(TokenType.CloseParen, 1);
                    ExitState();
                    return;
                case ',':
                    ConsumeToken(TokenType.Comma, 1);
                    return;
                default:
                    ScanCodeIdentifier();
                    return;

            }
        }
    }
}
