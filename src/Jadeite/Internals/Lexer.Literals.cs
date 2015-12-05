
using System;
using System.Diagnostics;

namespace Jadeite.Internals
{
    public partial class Lexer
    {

        // This method is lifted from the dotnet/roslyn C# parser with several modifications.
        // Original source: https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Portable/Parser/SlidingTextWindow.cs
        // Roslyn License (Apache 2.0) https://github.com/dotnet/roslyn/blob/master/License.txt
        private char ScanUnicodeEscape(out char surrogateCharacter)
        {
            surrogateCharacter = INVALID_CHAR;

            var c = CurrentChar();
            Debug.Assert(c == '\\');

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

                    uintChar = (uint)((uintChar << 4) + HexValue(c));
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

                c = (char)intChar;
            }

            return c;
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
            if (codepoint < (uint)0x00010000)
            {
                lowSurrogate = INVALID_CHAR;
                return (char)codepoint;
            }

            Debug.Assert(codepoint > 0x0000FFFF && codepoint <= 0x0010FFFF);
            lowSurrogate = (char)((codepoint - 0x00010000) % 0x0400 + 0xDC00);
            return (char)((codepoint - 0x00010000) / 0x0400 + 0xD800);
        }
    }
}
