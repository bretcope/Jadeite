
using System;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private Token ScanLineComment()
        {
            if (CurrentChar() != '/' || RelativeCharAt(1) != '/')
                return null;

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

            return ConsumeToken(TokenType.LineComment, valueLen, Input.Substring(Index + 2, valueLen - 2));
        }

        private Token ScanBlockComment()
        {
            if (CurrentChar() != '/' || RelativeCharAt(1) != '*')
                return null;

            var len = 4;
            var i = Index + 2;
            for (; i < Length; i++)
            {
                if (Input[i] == '*' && CharAt(i + 1) == '/')
                    break;

                len++;
            }

            if (i == Length)
                throw new Exception("Block comment was not terminated."); // todo

            return ConsumeToken(TokenType.BlockComment, len, Input.Substring(Index + 2, len - 4));
        }
    }
}