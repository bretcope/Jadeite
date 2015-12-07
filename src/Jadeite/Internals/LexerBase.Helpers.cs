
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Jadeite.Internals
{
    // Nothing in this file should affect the state of the lexer.
    public partial class LexerBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsWhiteSpace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsWhiteSpaceNewLineOrEnd(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                case INVALID_CHAR:
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsNewLineOrEnd(char c)
        {
            switch (c)
            {
                case '\r':
                case '\n':
                case INVALID_CHAR:
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsNewLine(char c)
        {
            switch (c)
            {
                case '\r':
                case '\n':
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsCodeIdentifierCharacter(char c)
        {
            return IsWordCharacter(c) || c == '$';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsWordCharacter(char c)
        {
            if (c >= 'a' && c <= 'z')
                return true;

            if (c >= 'A' && c <= 'Z')
                return true;

            if (c >= '0' && c <= '9')
                return true;

            if (c == '_')
                return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsWordOrHyphenCharacter(char c)
        {
            return IsWordCharacter(c) || c == '-';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsBinaryDigit(char c)
        {
            return c == '0' || c == '1';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsOctalDigit(char c)
        {
            return c == '0' || c == '7';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsDecimalDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsHexDigit(char c)
        {
            return IsDecimalDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int OctalValue(char c)
        {
            Debug.Assert(IsOctalDigit(c));
            return c - '0';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int DecimalValue(char c)
        {
            Debug.Assert(IsDecimalDigit(c));
            return c - '0';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int HexValue(char c)
        {
            Debug.Assert(IsHexDigit(c));
            return (c >= '0' && c <= '9') ? c - '0' : (c & 0xdf) - 'A' + 10;
        }
    }
}
