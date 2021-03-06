﻿using System.Diagnostics;
using System.Text;

namespace Jadeite.Parsing
{
    public partial class Lexer
    {
        private bool _htmlCommentAtInitialPosition;
        private int _htmlCommentIndent;
        private bool _htmlCommentBuffered;
        private readonly StringBuilder _htmlCommentBuilder = new StringBuilder();

        private void TransitionToHtmlComment()
        {
            Debug.Assert(CurrentChar() == '/' && NextChar() == '/');

            PushState(LexerState.HtmlComment);
            _htmlCommentAtInitialPosition = true;
            _htmlCommentIndent = IndentLevel + 1;
        }

        private void ScanHtmlComment()
        {
            var initial = _htmlCommentAtInitialPosition;
            var dex = Index;
            if (initial)
            {
                Debug.Assert(CurrentChar() == '/' && NextChar() == '/');

                dex += 2;
                if (CharAt(dex) == '-')
                {
                    dex++;
                    _htmlCommentBuffered = false;
                }
                else
                {
                    _htmlCommentBuffered = true;
                }

                _htmlCommentAtInitialPosition = false;
            }

            var sb = _htmlCommentBuilder;
            sb.Clear();
            switch (CharAt(dex))
            {
                case '\r':
                case '\n':
                    TransitionToIndent(_htmlCommentIndent);
                    break;
                case INVALID_CHAR:
                    ExitState();
                    return;
                default:
                    if (!initial && IndentLevel < _htmlCommentIndent)
                    {
                        ExitState();
                    }
                    else
                    {
                        for (; dex < Length; dex++)
                        {
                            var c = Input[dex];
                            if (IsNewLine(c))
                                break;

                            sb.Append(c);
                        }

                    }
                    break;
            }

            if (dex > Index)
                ConsumeToken(_htmlCommentBuffered ? JadeiteKind.BufferedHtmlComment : JadeiteKind.UnbufferedHtmlComment, dex - Index, sb.ToString());
        }

        private Token ScanCodeLineComment()
        {
            Debug.Assert(CurrentChar() == '/' && NextChar() == '/');

            var valueLen = 2;
            for (var i = Index + 2; i < Length; i++)
            {
                switch (Input[i])
                {
                    case '\n':
                    case '\r':
                        break;
                    default:
                        valueLen++;
                        continue;
                }

                break;
            }

            return ConsumeToken(JadeiteKind.CodeComment, valueLen, Input.Substring(Index + 2, valueLen - 2));
        }
    }
}