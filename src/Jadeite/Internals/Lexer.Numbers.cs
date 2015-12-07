using System;
using System.Diagnostics;
using System.Text;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private readonly StringBuilder _numberBuilder = new StringBuilder(16);

        private static readonly ulong[] _powersOfTen = {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000,
            10000000000,
            100000000000,
            1000000000000,
            10000000000000,
            100000000000000,
            1000000000000000,
            10000000000000000,
            100000000000000000,
            1000000000000000000,
            10000000000000000000,
        };

        private void ScanNumericLiteral()
        {
            Debug.Assert(IsDecimalDigit(CurrentChar()) || (CurrentChar() == '.' && IsDecimalDigit(NextChar())));

            if (CurrentChar() == '0')
            {
                switch (NextChar())
                {
                    case 'x':
                    case 'X':
                        ScanHexLiteral();
                        return;
                    case 'o':
                    case 'O':
                        ScanOctalLiteral();
                        return;
                    case 'b':
                    case 'B':
                        ScanBinaryLiteral();
                        return;
                }
            }

            ScanDecimalLiteral();
        }

        private void ScanBinaryLiteral()
        {
            Debug.Assert(CurrentChar() == '0' && (NextChar() == 'b' || NextChar() == 'B'));

            ulong value = 0;
            var dex = Index + 2;
            var bitsUsed = 0;
            for (; dex < Length; dex++)
            {
                var c = Input[dex];
                if (!IsBinaryDigit(c))
                    break;

                bitsUsed += 1;
                if (bitsUsed > 64)
                    throw new Exception($"Integer literal is larger than the maxium unsigned long. Line {Line} Column {Column}.");

                value = value << 1;
                if (c == '1')
                    value |= 1;
            }
            
            FinishIntegerScan(value, dex - Index - 2, dex);
        }

        private void ScanOctalLiteral()
        {
            Debug.Assert(CurrentChar() == '0' && (NextChar() == 'o' || NextChar() == 'O'));

            ulong value = 0;
            var dex = Index + 2;
            var bitsUsed = 0;
            for (; dex < Length; dex++)
            {
                var c = Input[dex];
                if (!IsOctalDigit(c))
                    break;

                bitsUsed += 3;
                if (bitsUsed > 64)
                    throw new Exception($"Integer literal is larger than the maxium unsigned long. Line {Line} Column {Column}.");

                value = (value << 3) | (ulong)OctalValue(c);
            }

            FinishIntegerScan(value, dex - Index - 2, dex);
        }

        private void ScanHexLiteral()
        {
            Debug.Assert(CurrentChar() == '0' && (NextChar() == 'x' || NextChar() == 'X'));

            ulong value = 0;
            var dex = Index + 2;
            var bitsUsed = 0;
            for (; dex < Length; dex++)
            {
                var c = Input[dex];
                if (!IsHexDigit(c))
                    break;

                bitsUsed += 4;
                if (bitsUsed > 64)
                    throw new Exception($"Integer literal is larger than the maxium unsigned long. Line {Line} Column {Column}.");

                value = (value << 4) | (ulong)HexValue(c);
            }

            FinishIntegerScan(value, dex - Index - 2, dex);
        }

        private void ScanDecimalLiteral()
        {
            Debug.Assert(IsDecimalDigit(CurrentChar()) || (CurrentChar() == '.' && IsDecimalDigit(NextChar())));

            var hasDotOrExponent = false;
            var sb = _numberBuilder;
            sb.Clear();
            var dex = Index;
            for (; dex < Length; dex++)
            {
                var c = Input[dex];
                if (IsDecimalDigit(c))
                {
                    sb.Append(c);
                }
                else if (c == '.')
                {
                    hasDotOrExponent = true;
                    sb.Append(c);
                }
                else if (c == 'e' || c == 'E')
                {
                    hasDotOrExponent = true;
                    sb.Append(c);

                    var next = CharAt(dex + 1);
                    if (next == '+' || next == '-')
                    {
                        sb.Append(next);
                        dex++;
                    }
                }
                else
                {
                    break;
                }
            }

            if (hasDotOrExponent)
            {
                FinishFloatingPointScan(sb, dex);
            }
            else // integer value
            {
                ulong value;
                if (sb.Length < 20)
                {
                    // definitely not going to have an overflow
                    var power = sb.Length - 1;
                    value = 0UL;
                    for (var i = 0; i < sb.Length; i++)
                    {
                        value += (ulong)DecimalValue(sb[i]) * _powersOfTen[power];
                        power--;
                    }
                }
                else
                {
                    // enough digits that there might be an overflow
                    if (!ulong.TryParse(sb.ToString(), out value))
                        throw new Exception($"Invalid numeric literal Line {Line} Column {Column}."); // todo

                }

                FinishIntegerScan(value, dex - Index, dex);
            }
        }

        private void FinishFloatingPointScan(StringBuilder text, int endIndex)
        {
            // floating point number
            var type = 'd';
            switch (CharAt(endIndex))
            {
                case 'f':
                case 'F':
                    type = 'f';
                    endIndex++;
                    break;
                case 'd':
                case 'D':
                    type = 'd';
                    endIndex++;
                    break;
                case 'm':
                case 'M':
                    type = 'm';
                    endIndex++;
                    break;
            }

            if (IsWordCharacter(CharAt(endIndex)))
                throw new Exception($"Invalid numeric literal Line {Line} Column {Column}."); // todo

            object value;

            if (type == 'd')
            {
                double d;
                if (!double.TryParse(text.ToString(), out d)) // todo intern strings coming out of the builder
                    throw new Exception($"Invalid numeric literal Line {Line} Column {Column}."); // todo

                value = d;
            }
            else if (type == 'f')
            {
                float f;
                if (!float.TryParse(text.ToString(), out f))
                    throw new Exception($"Invalid numeric literal Line {Line} Column {Column}."); // todo

                value = f;
            }
            else
            {
                throw new NotImplementedException($"Decimal types have not been implemented for Jadeite yet. Line {Line} Column {Column}.");
            }

            ConsumeToken(TokenType.FloatingPointLiteral, endIndex - Index, value);
        }

        private void FinishIntegerScan(ulong value, int digitCount, int endIndex)
        {
            if (digitCount == 0)
                throw new Exception($"Invalid numeric literal Line {Line} Column {Column}."); // todo

            // figure out the correct type
            int suffixLength;
            object finalVal = ULongToCorrectIntegerType(value, endIndex, out suffixLength);
            endIndex += suffixLength;

            if (IsWordCharacter(CharAt(endIndex)))
                throw new Exception($"Invalid numeric literal Line {Line} Column {Column}."); // todo

            ConsumeToken(TokenType.IntegerLiteral, endIndex - Index, finalVal);
        }

        private object ULongToCorrectIntegerType(ulong value, int suffixIndex, out int suffixLength)
        {
            switch (CharAt(suffixIndex))
            {
                case 'u':
                case 'U':
                    switch (CharAt(suffixIndex + 1))
                    {
                        case 'l':
                        case 'L':
                            suffixLength = 2;
                            return value;
                        default:
                            suffixLength = 1;
                            if (value <= uint.MaxValue)
                                return (uint)value;
                            return value;
                    }
                case 'l':
                case 'L':
                    switch (CharAt(suffixIndex + 1))
                    {
                        case 'u':
                        case 'U':
                            suffixLength = 2;
                            return value;
                        default:
                            suffixLength = 1;
                            if (value <= long.MaxValue)
                                return (long)value;
                            return value;
                    }
                default:
                    suffixLength = 0;

                    if (value < int.MaxValue)
                        return (int)value;

                    if (value < uint.MaxValue)
                        return (uint)value;

                    if (value < long.MaxValue)
                        return (long)value;

                    return value;
            }
        }
    }
}
