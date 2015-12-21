
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
                    ConsumeToken(JadeiteSyntaxKind.CloseParen, 1);
                    ExitState();
                    return;
                case ',':
                    ConsumeToken(JadeiteSyntaxKind.Comma, 1);
                    return;
                default:
                    ScanCodeIdentifier();
                    return;

            }
        }
    }
}
