
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
            switch (CurrentChar())
            {
                case '\r':
                case '\n':
                    TransitionToIndent(_pipelessIndent);
                    break;
                case INVALID_CHAR:
                    ExitState();
                    return;
                default:
                    if (IndentLevel < _pipelessIndent)
                    {
                        ExitState();
                    }
                    else
                    {
                        TransitionToBody(false);
                    }
                    break;
            }
        }
    }
}
