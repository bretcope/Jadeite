using System;
using System.Diagnostics;
using System.Text;

namespace Jadeite.Parsing
{
    public partial class Lexer
    {
        private readonly StringBuilder _stringLiteralBuilder = new StringBuilder();

        private void ScanCharacterLiteral()
        {
            Debug.Assert(CurrentChar() == '\'');

            ScanStringImpl(false, false);
        }

        private void ScanStringLiteral()
        {
            Debug.Assert(CurrentChar() == '"');

            ScanStringImpl(false, false);
        }

        private void ScanVerbatimString()
        {
            Debug.Assert(CurrentChar() == '@');
            Debug.Assert(NextChar() == '"');

            ScanStringImpl(true, false);
        }

        private void ScanInterpolatedString()
        {
            Debug.Assert(CurrentChar() == '$');
            Debug.Assert(NextChar() == '"' || (NextChar() == '@' && CharAt(Index + 2) == '"'));

            if (NextChar() == '"')
                ScanStringImpl(false, true);
            else 
                ScanStringImpl(true, true);
        }

        private void ScanStringImpl(bool isVerbatim, bool isInterpolated)
        {
            if (isInterpolated)
                throw new NotImplementedException($"String interpolation has not been implemented in Jadeite yet. Line {Line} Column {Column}.");
            
            var dex = Index;
            var c = CurrentChar();

            if (c == '@')
            {
                dex++;
                c = CharAt(dex);
            }
            else if (c == '$')
            {
                dex++;
                c = CharAt(dex);
                if (c == '@')
                {
                    dex++;
                    c = CharAt(dex);
                }
            }

            Debug.Assert(c == '"' || c == '\'');

            var quote = c;
            dex++;
            var sb = _stringLiteralBuilder;
            sb.Clear();

            for (; dex < Length; dex++)
            {
                c = Input[dex];
                switch (c)
                {
                    case '"':
                    case '\'':
                        if (c == quote)
                        {
                            if (isVerbatim && CharAt(dex + 1) == quote)
                            {
                                dex++;
                                sb.Append(quote);
                                break;
                            }

                            goto EXIT_LOOP;
                        }

                        goto default;
                    case '\\':
                        if (isVerbatim)
                            goto default;

                        char surrogate;
                        dex += ScanEscapeSequence(dex, out c, out surrogate);
                        sb.Append(c);
                        if (surrogate != INVALID_CHAR)
                        {
                            sb.Append(surrogate);
                        }
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            EXIT_LOOP:
            ;

            if (CharAt(dex) != quote)
                throw new Exception($"Unterminated literal at Line {Line} Column {Column}.");

            dex++; // consume the closing quote

            if (quote == '\'')
            {
                if (sb.Length != 1)
                    throw new Exception($"Invalid character literal at Line {Line} Column {Column}.");

                ConsumeToken(JadeiteSyntaxKind.CharLiteral, dex - Index, sb[0]);
            }
            else
            {
                ConsumeToken(JadeiteSyntaxKind.StringLiteral, dex - Index, sb.ToString());
            }

        }

        // This method is lifted from the dotnet/roslyn C# parser with several modifications.
        // Original source: https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Portable/Parser/Lexer_StringLiteral.cs
        // Roslyn License (Apache 2.0) https://github.com/dotnet/roslyn/blob/master/License.txt
        // Returns the number of characters after the backslash which were part of the escape sequence
        private int ScanEscapeSequence(int start, out char c, out char surrogateCharacter)
        {
            Debug.Assert(CharAt(start) == '\\');

            surrogateCharacter = INVALID_CHAR;
            c = CharAt(start + 1);
            switch (c)
            {
                // escaped characters that translate to themselves
                case '\'':
                case '"':
                case '\\':
                    return 1;
                case '0':
                    c = '\u0000';
                    return 1;
                case 'a':
                    c = '\u0007';
                    return 1;
                case 'b':
                    c = '\u0008';
                    return 1;
                case 'f':
                    c = '\u000c';
                    return 1;
                case 'n':
                    c = '\u000a';
                    return 1;
                case 'r':
                    c = '\u000d';
                    return 1;
                case 't':
                    c = '\u0009';
                    return 1;
                case 'v':
                    c = '\u000b';
                    return 1;
                case 'x':
                case 'u':
                case 'U':
                    return ScanUnicodeEscape(start, out c, out surrogateCharacter);
                default:
                    throw new Exception($"Illegal escape sequence at Line {Line} Column {Column}.");
            }
        }

        // This method is lifted from the dotnet/roslyn C# parser with several modifications.
        // Original source: https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Portable/Parser/SlidingTextWindow.cs
        // Roslyn License (Apache 2.0) https://github.com/dotnet/roslyn/blob/master/License.txt
        // Returns the number of characters after the backslash which were part of the escape sequence
        private int ScanUnicodeEscape(int start, out char c, out char surrogateCharacter)
        {
            Debug.Assert(CharAt(start) == '\\');

            surrogateCharacter = INVALID_CHAR;
            var dex = Index + 1;
            c = CharAt(dex);
            dex++;
            if (c == 'U')
            {
                uint uintChar = 0;

                if (!IsHexDigit(CharAt(dex)))
                    throw CreateIllegalUnicodeEscapeException();

                for (var i = 0; i < 8; i++)
                {
                    c = CharAt(dex);
                    if (!IsHexDigit(c))
                        throw CreateIllegalUnicodeEscapeException();

                    uintChar = (uint) ((uintChar << 4) + HexValue(c));
                    dex++;
                }

                if (uintChar > 0x0010FFFF)
                    throw CreateIllegalUnicodeEscapeException();

                c = GetCharsFromUtf32(uintChar, out surrogateCharacter);
            }
            else
            {
                Debug.Assert(c == 'u' || c == 'x');

                int intChar = 0;
                if (!IsHexDigit(CharAt(dex)))
                    throw CreateIllegalUnicodeEscapeException();

                for (var i = 0; i < 4; i++)
                {
                    var ch2 = CharAt(dex);
                    if (!IsHexDigit(ch2))
                    {
                        if (c == 'u')
                            throw CreateIllegalUnicodeEscapeException();

                        break;
                    }

                    intChar = (intChar << 4) + HexValue(ch2);
                    dex++;
                }

                c = (char) intChar;
            }

            return dex - Index - 1;
        }

        private Exception CreateIllegalUnicodeEscapeException()
        {
            return new Exception($"Illegal unicode escape sequence. Line {Line} Column {Column}."); // todo
        }

        // This method is lifted from the dotnet/roslyn C# parser with several modifications.
        // Original source: https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Portable/Parser/SlidingTextWindow.cs
        // Roslyn License (Apache 2.0) https://github.com/dotnet/roslyn/blob/master/License.txt
        private static char GetCharsFromUtf32(uint codepoint, out char lowSurrogate)
        {
            if (codepoint < (uint) 0x00010000)
            {
                lowSurrogate = INVALID_CHAR;
                return (char) codepoint;
            }

            Debug.Assert(codepoint > 0x0000FFFF && codepoint <= 0x0010FFFF);
            lowSurrogate = (char) ((codepoint - 0x00010000) % 0x0400 + 0xDC00);
            return (char) ((codepoint - 0x00010000) / 0x0400 + 0xD800);
        }
    }
}
