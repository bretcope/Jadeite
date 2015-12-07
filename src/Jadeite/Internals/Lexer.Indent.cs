using System;
using System.Linq;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private enum IndentType : byte
        {
            Tabs,
            Spaces,
        }

        private int _maxIndent;
        private IndentType _indentType;
        private int _indentCharCount;

        private int IndentLevel { get; set; }

        private void TransitionToIndent(int maxIndent = int.MaxValue)
        {
            _maxIndent = maxIndent;
            PushState(LexerState.Indent);
        }

        private void ScanIndent()
        {
            switch (CurrentChar())
            {
                case '\r':
                    ConsumeToken(TokenType.NewLine, NextChar() == '\n' ? 2 : 1);
                    IndentLevel = 0;
                    return;
                case '\n':
                    ConsumeToken(TokenType.NewLine, 1);
                    IndentLevel = 0;
                    return;
                case INVALID_CHAR:
                    ExitState();
                    return;
            }

            var charCount = 0;
            var indentChar = _indentType == IndentType.Tabs ? '\t' : ' ';

            var i = Index;
            while (CharAt(i) == indentChar)
            {
                charCount++;
                i++;
            }

            var endOfIndentIndex = i;

            // make sure the line isn't just empty
            while (IsWhiteSpace(CharAt(i)))
            {
                i++;
            }

            switch (CharAt(i))
            {
                case '\r':
                case '\n':
                case INVALID_CHAR:
                    // line only contains white space - just consume it as trivia and move on
                    ConsumeTrivia(i - Index);
                    return;
            }

            // got a real indent
            if (_maxIndent < int.MaxValue)
            {
                // we're in a limited indent mode
                var maxCharCount = _maxIndent * _indentCharCount;
                if (charCount > maxCharCount)
                    charCount = maxCharCount;
            }
            else if (i > endOfIndentIndex)
            {
                // When not in a limited indent mode, the character that follows shouldn't be white space.
                // If it is, it means someone is accidentally mixing tabs and spaces.
                switch (CharAt(endOfIndentIndex))
                {
                    case ' ':
                    case '\t':
                        throw new Exception($"Mixing tabs and spaces on line {Line}."); // todo
                }
            }

            if (_indentType == IndentType.Spaces && charCount % _indentCharCount != 0)
            {
                throw new Exception($"Indent on line {Line} is not a multiple of {_indentCharCount} spaces.");
            }

            // actually consume the indents
            var newIndent = charCount / _indentCharCount;
            for (var il = 0; il < newIndent; il++)
            {
                ConsumeToken(TokenType.Indent, _indentCharCount);
            }

            IndentLevel = newIndent;
            ExitState();
        }

        private void SetIndent(string indent)
        {
            if (indent == "\t")
            {
                _indentType = IndentType.Tabs;
                _indentCharCount = 1;
            }
            else
            {
                if (indent == null || indent.Length < 2 || indent.Length > 8 || indent.Any(c => c != ' '))
                    throw new Exception("Indent must be either a single tab, or between 2 and 8 spaces.");

                _indentType = IndentType.Spaces;
                _indentCharCount = indent.Length;
            }
        }
    }
}