
using System;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private void TransitionToBody()
        {
            PushState(LexerState.Body);
        }

        private void ScanBody()
        {
            throw new NotImplementedException();
        }
    }
}
