using System;

namespace Jadeite.Internals
{
    public class Lexer : LexerBase
    {
        private delegate LexMethod LexMethod();

        private LexMethod _lexMethod;

        public Lexer(string input) : base(input)
        {
            _lexMethod = LexHeader;
        }

        protected override void Lex()
        {
            if (ConsumeOptionalToken(TokenType.EndOfInput))
                return;

            if (ConsumeOptionalToken(TokenType.NewLine))
            {
                // lex method resets every time there's a new line
                _lexMethod = LexLineStart;

                // we also exit any interpolation context, because those never span lines
                // if we have to exit here, that's going to be a syntax error, but we'll let the parser deal with that
                while (true)
                {
                    switch (Context)
                    {
                        case LexerContext.TagInterpolation:
                        case LexerContext.Code:
                            ExitContext();
                            continue;
                    }

                    break;
                }
            }
            else if (_lexMethod == null)
            {
                throw new Exception("Why is _lexMethod null and there was no new line?"); // todo
            }

            _lexMethod = _lexMethod();
        }

        private LexMethod LexHeader()
        {
            ScanIndentDefinition();
            ConsumeToken(TokenType.Model);
            ConsumeToken(TokenType.Identifier);
            // deal with insignificant whitespace
            //ConsumeToken();

            // new line
        }

        private LexMethod LexLineStart()
        {
            var wsEnd = GetEndOfWhiteSpaceOnLine();
            var wsLength = wsEnd - _index;

            // check if the line is entirely white space
            switch (CharAt(wsEnd))
            {
                case '\n':
                case '\r':
                    CreateToken(TokenType.BlankLine, wsLength);
                    return null;
                case INVALID_CHAR:
                    if (_index != wsEnd) // don't need to emit a blank line if this is end of file and there's not even white space on the line
                        CreateToken(TokenType.BlankLine, wsLength);

                    return null;
            }

            // figure out the indent situation
            var inPlainTextMode = Context == LexerContext.PlainTextBlock;
            var adjustedLength = wsLength;
            var adjustedEnd = wsEnd;
            if (inPlainTextMode)
            {
                // we don't want to consider anything past the plain text indent level as an indent (it could be white space the user wants)
                var maxChars = PlainTextIndentLevel * _indentCharCount;
                if (adjustedLength > maxChars)
                {
                    adjustedLength = maxChars;
                    adjustedEnd = _index + adjustedLength;
                }
            }

            var indentChar = _indentMode == IndentMode.Spaces ? ' ' : '\t';
            for (var i = _index; i < adjustedEnd; i++)
            {
                if (CharAt(i) != indentChar)
                    throw new Exception("Cannot mix tabs and spaces."); // todo
            }

            // make sure it's a multiple of the char count
            if (_indentMode == IndentMode.Spaces && adjustedLength % _indentCharCount != 0)
            {
                throw new Exception($"Indentation was not a multiple of {_indentCharCount} spaces."); // todo
            }

            // setup the new indent level, and create indent/outdent tokens
            var newIndent = adjustedLength / _indentCharCount;

            if (newIndent > _indentLevel)
            {
                ConsumeLeadingTrivia(_indentLevel * _indentCharCount);
                do
                {
                    CreateToken(TokenType.Indent, _indentCharCount);
                    _indentLevel++;

                } while (newIndent > _indentLevel);
            }
            else
            {
                ConsumeLeadingTrivia(adjustedLength);

                while (newIndent < _indentLevel)
                {
                    CreateToken(TokenType.Outdent, 0);
                    _indentLevel--;
                }

                if (inPlainTextMode && _indentLevel < PlainTextIndentLevel)
                    ExitContext();
            }

            return LexContentStart;
        }

        private int GetEndOfWhiteSpaceOnLine()
        {
            var whiteSpaceEnd = _index;
            while (whiteSpaceEnd < _length)
            {
                switch (Input[whiteSpaceEnd])
                {
                    case ' ':
                    case '\t':
                        whiteSpaceEnd++;
                        continue;
                }

                break;
            }

            return whiteSpaceEnd;
        }

        private void ScanIndentDefinition()
        {
            var whiteSpaceEnd = GetEndOfWhiteSpaceOnLine();
            if (whiteSpaceEnd == _index)
                throw new Exception("Indentation not set on first line."); // todo

            var c = CurrentCharacter();
            if (c == '\t')
            {
                if (whiteSpaceEnd - _index != 1)
                    throw new Exception("Only a single tab character can be used as an indent. It cannot be combined with other characters."); // todo

                _indentMode = IndentMode.Tabs;
                _indentCharCount = 1;

                CreateToken(TokenType.IndentDefinition, 1);
            }
            else if (c == ' ')
            {
                var count = whiteSpaceEnd - _index;
                if (count > 8)
                    throw new Exception("Cannot use more than eight spaces as the indentation.");

                // make sure all chars are spaces
                for (var i = _index + 1; i < whiteSpaceEnd; i++)
                {
                    if (CharAt(i) != ' ')
                        throw new Exception("Cannot mix tabs and spaces in the indentation definition."); // todo
                }

                _indentMode = IndentMode.Spaces;
                _indentCharCount = count;

                CreateToken(TokenType.IndentDefinition, count);
            }

            throw new Exception("There's a whitespace definition mismatch between ScanIndentDefinition and GetEndOfWhiteSpaceOnLine.");
        }

        private LexMethod LexContentStart()
        {
            //
        }

        private LexMethod LexLineComment()
        {
            //
        }

        private LexMethod ScanBlockComment()
        {
            //
        }

        private void ScanTagBody()
        {
            //
        }
    }
}