
using System;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private int _pipelessIndent;

        private void TransitionToPipelessText()
        {
            PushState(LexerState.PipelessText);
            _pipelessIndent = IndentLevel + 1;
        }

        private void ScanPipelessText()
        {
            throw new NotImplementedException();
        }
    }
}
