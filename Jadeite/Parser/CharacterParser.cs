using System.Linq;
using System.Text.RegularExpressions;

namespace Jadeite.Parser
{
    public static class CharacterParser
    {
        /// <summary>
        /// Parses the source until the first unmatched close bracket (any of ), }, ])
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start">Index to begin searching the string.</param>
        /// <returns>The index of the unmatched bracket. If none is found, -1 is returned.</returns>
        public static int FindNextUnmatchedBracket(string str, int start = 0)
        {
            var state = new CharacterParserState();
            var i = start;
            while (state.RoundDepth >= 0 && state.CurlyDepth >= 0 && state.SquareDepth >= 0)
            {
                if (i >= str.Length)
                    return -1;

                state.ParseCharacter(str[i++]);
            }

            return i - 1;
        }

        public static bool IsPunctuator(char c)
        {
            switch (c)
            {
                case '\0':
                case '.':
                case '(':
                case ')':
                case ';':
                case ',':
                case '{':
                case '}':
                case '[':
                case ']':
                case ':':
                case '?':
                case '~':
                case '%':
                case '&':
                case '*':
                case '+':
                case '-':
                case '/':
                case '<':
                case '>':
                case '^':
                case '|':
                case '!':
                case '=':
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsKeyword(string str)
        {
            switch (str)
            {
                case "if":
                case "in":
                case "do":
                case "var":
                case "for":
                case "new":
                case "try":
                case "let":
                case "this":
                case "else":
                case "case":
                case "void":
                case "with":
                case "enum":
                case "while":
                case "break":
                case "catch":
                case "throw":
                case "const":
                case "yield":
                case "class":
                case "super":
                case "return":
                case "typeof":
                case "delete":
                case "switch":
                case "export":
                case "import":
                case "default":
                case "finally":
                case "extends":
                case "function":
                case "continue":
                case "debugger":
                case "package":
                case "private":
                case "interface":
                case "instanceof":
                case "implements":
                case "protected":
                case "public":
                case "static":
                    return true;
                default:
                    return false;
            }
        }
    }

    public class CharacterParserState
    {
        public bool LineComment { get; set; }
        public bool BlockComment { get; set; }

        public bool SingleQuote { get; set; }
        public bool DoubleQuote { get; set; }
        public bool Escaped { get; set; }

        public bool Regex { get; set; }
        public bool RegexStart { get; set; }

        public int RoundDepth { get; set; }
        public int CurlyDepth { get; set; }
        public int SquareDepth { get; set; }

        public string History { get; set; } = "";
        public char LastChar { get; set; } = '\0';

        public bool IsString => SingleQuote || DoubleQuote;
        public bool IsComment => LineComment || BlockComment;
        public bool IsNesting => IsString || IsComment || Regex || RoundDepth > 0 || CurlyDepth > 0 || SquareDepth > 0;

        public void ParseCharacter(char c)
        {
            var wasComment = BlockComment || LineComment;
            var lastChar = History != "" ? History[0] : '\0';

            if (RegexStart)
            {
                if (c == '/' || c == '*')
                    Regex = false;

                RegexStart = false;
            }

            if (LineComment)
            {
                if (c == '\n')
                    LineComment = false;
            }
            else if (BlockComment)
            {
                if (LastChar == '*' && c == '/')
                    BlockComment = false;
            }
            else if (SingleQuote)
            {
                if (c == '\'' && !Escaped)
                    SingleQuote = false;
                else
                    Escaped = c == '\\' && !Escaped;
            }
            else if (DoubleQuote)
            {
                if (c == '"' && !Escaped)
                    DoubleQuote = false;
                else
                    Escaped = c == '\\' && !Escaped;
            }
            else if (Regex)
            {
                if (c == '/' && !Escaped)
                    Regex = false;
                else
                    Escaped = c == '\\' && !Escaped;
            }
            else if (lastChar == '/' && c == '/')
            {
                History = History.Substring(1);
                LineComment = true;
            }
            else if (lastChar == '/' && c == '*')
            {
                History = History.Substring(1);
                BlockComment = true;
            }
            else if (c == '/' && IsRegex(History))
            {
                Regex = true;
                RegexStart = true;
            }
            else if (c == '\'')
            {
                SingleQuote = true;
            }
            else if (c == '"')
            {
                DoubleQuote = true;
            }
            else if (c == '(')
            {
                RoundDepth++;
            }
            else if (c == ')')
            {
                RoundDepth--;
            }
            else if (c == '{')
            {
                CurlyDepth++;
            }
            else if (c == '}')
            {
                CurlyDepth--;
            }
            else if (c == '[')
            {
                SquareDepth++;
            }
            else if (c == ']')
            {
                SquareDepth--;
            }

            if (!BlockComment && !LineComment && !wasComment)
                History = c + History;

            LastChar = c; // store last character for ending block comments
        }

        private static readonly Regex s_WordBoundaryRegex = new Regex(@"^\w+\b");
        private static bool IsRegex(string history)
        {
            history = history.TrimStart();

            // unless its an `if`, `while`, `for` or `with` it's a divide, so we assume it's a divide
            if (history[0] == ')')
                return false;

            // unless it's a function expression, it's a regexp, so we assume it's a regexp
            if (history[0] == '}')
                return true;

            // any punctuation means it's a regexp
            if (CharacterParser.IsPunctuator(history[0]))
                return true;

            // if the last thing was a keyword then it must be a regexp (e.g. `typeof /foo/`)
            var match = s_WordBoundaryRegex.Match(history);
            return match.Success && CharacterParser.IsKeyword(new string(match.Value.Reverse().ToArray()));
        }
    }
}