using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private readonly Stack<bool> _isBodyInterpolationStack = new Stack<bool>();
        private readonly StringBuilder _bodyStringBuilder = new StringBuilder();

        private void TransitionToBody(bool isInterpolation)
        {
            PushState(LexerState.Body);
            _isBodyInterpolationStack.Push(isInterpolation);
        }

        private void OnExitBody()
        {
            _isBodyInterpolationStack.Pop();
        }

        private void ScanBody()
        {
            var sb = _bodyStringBuilder;
            Debug.Assert(sb.Length == 0);

            var isInterpolation = _isBodyInterpolationStack.Peek();
            var i = Index;
            while(i < Length)
            {
                var c = Input[i];
                switch (c)
                {
                    case '\r':
                    case '\n':
                        goto EXIT_LOOP;
                    case ']':
                        if (isInterpolation)
                        {
                            if (i == Index)
                            {
                                ConsumeToken(TokenType.CloseSquareBracket, 1);
                                ExitState();
                                return;
                            }

                            goto EXIT_LOOP;
                        }
                        goto default;
                    case '#':
                        var nextChar = CharAt(i + 1);
                        if (nextChar == '{')
                        {
                            if (i == Index)
                            {
                                ConsumeToken(TokenType.OpenEscapedInterpolation, 2);
                                TransitionToCode(CodeScanMode.CurlyInterpolation);
                                return;
                            }

                            goto EXIT_LOOP;
                        }

                        if (nextChar == '[')
                        {
                            if (i == Index)
                            {
                                ConsumeToken(TokenType.OpenTagInterpolation, 2);
                                TransitionToTag(isInterpolation: true);
                                return;
                            }

                            goto EXIT_LOOP;
                        }

                        goto default;
                    case '!':
                        if (CharAt(i + 1) == '{')
                        {
                            if (i == Index)
                            {
                                ConsumeToken(TokenType.OpenNonEscapedInterpolation, 2);
                                TransitionToCode(CodeScanMode.CurlyInterpolation);
                                return;
                            }

                            goto EXIT_LOOP;
                        }
                        goto default;
                    case '\\':
                        i += ScanPossibleBodyEscapeSequence(i, isInterpolation, sb);
                        break;
                    default:
                        sb.Append(c);
                        i++;
                        break;
                }
            }

            EXIT_LOOP:
            ;

            if (i == Index)
            {
                // didn't consume anything - remaining in this state would result in an infinite loop
                ExitState();
                return;
            }

            ConsumeToken(TokenType.BodyText, i - Index, sb.ToString());
            sb.Clear();
        }

        // returns the number of characters consumed
        private int ScanPossibleBodyEscapeSequence(int start, bool isInterpolation, StringBuilder sb)
        {
            Debug.Assert(CharAt(start) == '\\');

            var escapeCount = 1;
            var dex = start + 1;
            for (; dex < Length; dex++)
            {
                if (Input[dex] == '\\')
                    escapeCount++;
                else 
                    break;
            }

            var treatAsEscape = false;
            switch (CharAt(dex))
            {
                case '#':
                    var nextChar = CharAt(dex + 1);
                    if (nextChar == '[' || nextChar == '{')
                        treatAsEscape = true;
                    break;
                case '!':
                    if (CharAt(dex + 1) == '{')
                        treatAsEscape = true;
                    break;
                case ']':
                    treatAsEscape = isInterpolation;
                    break;
            }

            if (treatAsEscape)
            {
                var literalCount = escapeCount / 2;
                for (var i = 0; i < literalCount; i++)
                {
                    sb.Append('\\');
                }

                // escapeCount % 2 == 1 means the following character is escaped
                // If we're not at the end of input, we want to consume it as a literal character
                if (escapeCount % 2 == 1 && dex < Length)
                {
                    sb.Append(Input[dex]);
                    escapeCount++;
                }
            }
            else
            {
                // these are just literal backslashes
                for (var i = 0; i < escapeCount; i++)
                {
                    sb.Append('\\');
                }
            }

            return escapeCount;
        }
    }
}
