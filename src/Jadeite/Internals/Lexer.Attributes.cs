
using System;

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
            throw new NotImplementedException();
        }
    }
}
