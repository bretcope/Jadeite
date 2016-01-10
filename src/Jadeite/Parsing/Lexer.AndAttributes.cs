
namespace Jadeite.Parsing
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
                    ConsumeToken(JadeiteKind.CloseParen, 1);
                    ExitState();
                    return;
                case ',':
                    ConsumeToken(JadeiteKind.Comma, 1);
                    return;
                default:
                    ScanCodeIdentifier();
                    return;

            }
        }
    }
}
