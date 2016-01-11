
namespace Jadeite.Parsing
{
    public partial class Lexer
    {
        private int _pipelessIndent;
        private bool _pipelessAtInitialPosition;

        private void TransitionToPipelessText()
        {
            PushState(LexerState.PipelessText);
            _pipelessIndent = IndentLevel + 1;
            _pipelessAtInitialPosition = true;
        }

        private void ScanPipelessText()
        {
            switch (CurrentChar())
            {
                case '\r':
                case '\n':
                    var i = Index;

                    if (_pipelessAtInitialPosition)
                    {
                        // the first new line is consumed as an end of line token, all others are consumed as literal text
                        if (CurrentChar() == '\r' && NextChar() == '\n')
                            ConsumeToken(JadeiteKind.EndOfLine, 2);
                        else
                            ConsumeToken(JadeiteKind.EndOfLine, 1);

                        _pipelessAtInitialPosition = false;
                    }
                    else
                    {
                        i++;
                    }

                    while (IsNewLine(CharAt(i)))
                        i++;

                    if (i > Index)
                        ConsumeToken(JadeiteKind.HtmlText, i - Index, useTextAsValue: true);

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
