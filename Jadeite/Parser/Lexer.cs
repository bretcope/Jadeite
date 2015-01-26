using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jadeite.Parser
{
    public class Lexer
    {
        private readonly string _input;
        private readonly string _filename;

        private readonly Queue<Token> _deferredTokens = new Queue<Token>();
        private int _lineNumber = 1;
        private readonly List<Token> _stash = new List<Token>();
        private readonly Stack<int> _indentStack = new Stack<int>();
        private Regex _indentRegex;
        private bool _pipeless = false;
        private int _inputPosition = 0;

        public Lexer(string input, string filename)
        {
            _input = NormalizeInput(input);
            _filename = filename;
        }

        private string NormalizeInput(string input)
        {
            // remove BOM
//            if (input.StartsWith("\uFEFF"))
//                input = input.Substring("\uFEFF".Length);

            // normalize line endings
            return input.Replace("\r\n", "\n");
        }

        private char CurrentChar => _inputPosition < _input.Length ? _input[_inputPosition] : '\0';

        private static void AssertExpression(string exp)
        {
            //this verifies that a C# expression is valid
            ValidateSyntaxNode(SyntaxFactory.ParseExpression(exp));
        }

        private static void ValidateSyntaxNode(SyntaxNode node)
        {
            if (node.ContainsSkippedText)
                throw new Exception("Skipped text");

            foreach (var n in node.ChildNodes())
            {
                if (n.ContainsSkippedText)
                    throw new Exception("Skipped text");

                ValidateSyntaxNode(n);
            }

            foreach (var t in node.ChildTokens())
            {
                if (t.IsMissing)
                    throw new Exception("Missing Token");
            }
        }

        private static void AssertNestingCorrect(string exp)
        {
            // todo - in jade: 
            //this verifies that code is properly nested, but allows
            //invalid JavaScript such as the contents of `attributes`
        }

        private T CreateToken<T>(string value = null) where T : Token
        {
            var token = Activator.CreateInstance<T>();
            token.LineNumber = _lineNumber;
            token.Value = value;

            return token;
        }

        // "consume" the given `length` of input
        private void Consume(int length)
        {
            _inputPosition += length;
        }

        // Scan for token type with the given regex
        private T Scan<T>(Regex regex, bool captureValue = true) where T : Token
        {
            var match = regex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);
            var t = CreateToken<T>();
            if (captureValue)
                t.Value = match.Groups[1].Value;

            return t;
        }

        // Defer the given `tok`.
        private void Defer(Token t)
        {
            _deferredTokens.Enqueue(t);
        }

        // Lookahead `n` tokens.
        private Token LookAhead(int n)
        {
            var fetch = n - _stash.Count;
            while (fetch-- > 0)
            {
                _stash.Add(Next());
            }
            return _stash[--n];
        }

        private StringRange BracketExpression(int skip = 0)
        {
            var startIndex = skip + _inputPosition;
            var start = _input[startIndex];
            char end;
            switch (start)
            {
                case '(':
                    end = ')';
                    break;
                case '{':
                    end = '}';
                    break;
                case '[':
                    end = ']';
                    break;
                default:
                    throw new JadeiteParserException(_lineNumber, "Unrecognized start character '" + start + "'.");
            }

            var endIndex = CharacterParser.FindNextUnmatchedBracket(_input, startIndex + 1);
            if (endIndex == -1 || _input[endIndex] != end)
                throw new JadeiteParserException(_lineNumber, "No matching closing character found for '" + start + "'.");

            return new StringRange(_input, startIndex + 1, endIndex);
        }

        // Gets stashed token FIFO
        private Token Stashed()
        {
            if (_stash.Count > 0)
            {
                var t = _stash[0];
                _stash.RemoveAt(0);
                return t;
            }

            return null;
        }

        // Gets deferred token FIFO
        private Token Deferred()
        {
            return _deferredTokens.Count > 0 ? _deferredTokens.Dequeue() : null;
        }

        private Token EndOfSource()
        {
            if (_inputPosition < _input.Length)
                return null;

            if (_indentStack.Count > 0)
            {
                _indentStack.Pop();
                return CreateToken<OutdentToken>();
            }

            return CreateToken<EndOfSourceToken>();
        }

        private static readonly Regex s_BlankLineRegex = new Regex(@"\G\n *\n");
        private Token Blank()
        {
            var match = s_BlankLineRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length - 1);
            _lineNumber++;
            if (_pipeless)
                return CreateToken<TextToken>("");

            return Next();
        }

        private static readonly Regex s_CommentRegex = new Regex(@"\G//(-)?([^\n]*)");
        private CommentToken Comment()
        {
            var match = s_CommentRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);
            _pipeless = true;

            var t = CreateToken<CommentToken>(match.Groups[2].Value);
            t.Buffer = match.Groups[1].Success;
            return t;
        }

        private static readonly Regex s_InterpolationRegex = new Regex(@"\G#\{");
        private InterpolationToken Interpolation()
        {
            if (!s_InterpolationRegex.IsMatch(_input, _inputPosition))
                return null;

            var range = BracketExpression(1);
            Consume(range.End - _inputPosition);
            return CreateToken<InterpolationToken>(range.Value);
        }

        private static readonly Regex s_TagRegex = new Regex(@"\G(\w[-:\w]*)(/)?");
        private TagToken Tag()
        {
            var match = s_TagRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            TagToken t;
            var name = match.Groups[1].Value;
            if (name.EndsWith(":"))
            {
                name = name.Substring(0, name.Length - 1);
                t = CreateToken<TagToken>(name);
                Defer(CreateToken<ColonToken>());
                if (CurrentChar != ' ')
                    Debug.WriteLine("Warning: space required after `:` on line {0} of jade file \"{1}\"", _lineNumber, _filename);

                while (CurrentChar == ' ')
                {
                    Consume(1);
                }
            }
            else
            {
                t = CreateToken<TagToken>(name);
            }

            t.SelfClosing = match.Groups[2].Success;
            return t;
        }

        private static readonly Regex s_FilterRegex = new Regex(@"\G:([\w\-]+)");
        private FilterToken Filter()
        {
            var t = Scan<FilterToken>(s_FilterRegex);
            if (t != null)
                _pipeless = true;

            return t;
        }

        private static readonly Regex s_DoctypeRegex = new Regex(@"\G(?:doctype) *([^\n]+)?");
        private DocTypeToken DocType()
        {
            return Scan<DocTypeToken>(s_DoctypeRegex);
        }

        private static readonly Regex s_IdRegex = new Regex(@"\G#([\w-]+)");
        private IdToken Id()
        {
            return Scan<IdToken>(s_IdRegex);
        }

        private static readonly Regex s_ClassRegex = new Regex(@"\G\.([\w-]+)");
        private ClassToken Class()
        {
            return Scan<ClassToken>(s_ClassRegex);
        }

        private static readonly Regex s_TextRegex1 = new Regex(@"\G(?:\| ?| )([^\n]+)");
        private static readonly Regex s_TextRegex2 = new Regex(@"\G\|?( )");
        private static readonly Regex s_TextRegex3 = new Regex(@"\G(<[^\n]*)");
        private TextToken Text()
        {
            return Scan<TextToken>(s_TextRegex1) ??
                Scan<TextToken>(s_TextRegex2) ??
                Scan<TextToken>(s_TextRegex3);
        }

        private static readonly Regex s_TextFailRegex = new Regex(@"\G([^\.\n][^\n]+)");
        private TextToken TextFail()
        {
            var t = Scan<TextToken>(s_TextFailRegex);
            if (t != null)
                Debug.WriteLine("Warning: missing space before text for line {0}  of jade file \"{1}\"", _lineNumber, _filename);

            return t;
        }

        private static readonly Regex s_DotRegex = new Regex(@"\G\.");
        private DotToken Dot()
        {
            var t = Scan<DotToken>(s_DotRegex, false);
            if (t != null)
                _pipeless = true;

            return t;
        }

        private static readonly Regex s_ExtendsRegex = new Regex(@"\Gextends? +([^\n]+)");
        private ExtendsToken Extends()
        {
            return Scan<ExtendsToken>(s_ExtendsRegex);
        }

        private static readonly Regex s_PrependRegex = new Regex(@"\Gprepend +([^\n]+)");
        private BlockToken Prepend()
        {
            var match = s_PrependRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);
            var t = CreateToken<BlockToken>(match.Groups[1].Value);
            t.Mode = BlockMode.Prepend;
            return t;
        }

        private static readonly Regex s_AppendRegex = new Regex(@"\Gappend +([^\n]+)");
        private BlockToken Append()
        {
            var match = s_AppendRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);
            var t = CreateToken<BlockToken>(match.Groups[1].Value);
            t.Mode = BlockMode.Append;
            return t;
        }

        private static readonly Regex s_BlockRegex = new Regex(@"\Gblock\b *(?:(prepend|append) +)?([^\n]+)");
        private BlockToken Block()
        {
            var match = s_BlockRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            var t = CreateToken<BlockToken>(match.Groups[2].Value);
            switch (match.Groups[1].Value)
            {
                case "prepend":
                    t.Mode = BlockMode.Prepend;
                    break;
                case "append":
                    t.Mode = BlockMode.Append;
                    break;
                default:
                    t.Mode = BlockMode.Replace;
                    break;
            }

            return t;
        }

        private static readonly Regex s_MixinBlockRegex = new Regex(@"\Gblock[ \t]*(\n|$)");
        private MixinBlockToken MixinBlock()
        {
            var match = s_MixinBlockRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length - match.Groups[1].Length);
            return CreateToken<MixinBlockToken>();
        }

        private static readonly Regex s_YieldRegex = new Regex(@"\Gyield *");
        private YieldToken Yield()
        {
            return Scan<YieldToken>(s_YieldRegex, false);
        }

        private static readonly Regex s_IncludeRegex = new Regex(@"\Ginclude +([^\n]+)");
        private IncludeToken Include()
        {
            return Scan<IncludeToken>(s_IncludeRegex);
        }

        private static readonly Regex s_IncludeFilteredRegex = new Regex(@"\Ginclude:([\w\-]+)([\( ])");
        private static readonly Regex s_IncludeFilteredPathRegex = new Regex(@"\G *([^\n]+)");
        private IncludeToken IncludeFiltered()
        {
            var match = s_IncludeFilteredRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length - 1);
            var filter = match.Groups[1].Value;
            var attributes = match.Groups[2].Value == "(" ? Attributes() : null;
            if (match.Groups[2].Value != " " && CurrentChar != ' ')
                throw new JadeiteParserException(_lineNumber, "Expected space after include:filter but got '" + CurrentChar + "'");

            match = s_IncludeFilteredPathRegex.Match(_input, _inputPosition);
            if (!match.Success || match.Groups[1].Value.Trim() == "")
                throw new JadeiteParserException(_lineNumber, "Missing path for include:filter");

            Consume(match.Length);

            var path = match.Groups[1].Value;
            var t = CreateToken<IncludeToken>(path);
            t.Filter = filter;
            t.Attributes = attributes;
            return t;
        }

        private static readonly Regex s_CaseRegex = new Regex(@"\Gcase +([^\n]+)");
        private CaseToken Case()
        {
            return Scan<CaseToken>(s_CaseRegex);
        }

        private static readonly Regex s_WhenRegex = new Regex(@"\Gwhen +([^:\n]+)");
        private WhenToken When()
        {
            return Scan<WhenToken>(s_WhenRegex);
        }

        private static readonly Regex s_DefaultRegex = new Regex(@"\Gdefault *");
        private DefaultToken Default()
        {
            return Scan<DefaultToken>(s_DefaultRegex, false);
        }

        private static readonly Regex s_CallRegex = new Regex(@"\G\+(\s*)(([-\w]+)|(#\{))");
        private static readonly Regex s_CallOpenParenRegex = new Regex(@"\G *\(");
        private static readonly Regex s_CallArgsRegex = new Regex(@"\G\s*[-\w]+ *=");
        private CallToken Call()
        {
            var match = s_CallRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            CallToken t;

            // try to consume simple or interpolated call
            if (match.Groups[3].Success)
            {
                // simple call
                Consume(match.Length);
                t = CreateToken<CallToken>(match.Groups[3].Value);
            }
            else
            {
                // interpolated call
                var range = BracketExpression(2 + match.Groups[1].Length);
                Consume(range.End + 1);
                AssertExpression(range.Value);
                t = CreateToken<CallToken>(range.Value);
            }

            // Check for args (not attributes)
            match = s_CallOpenParenRegex.Match(_input, _inputPosition);
            if (match.Success)
            {
                var range = BracketExpression(match.Length - 1);
                if (s_CallArgsRegex.IsMatch(range.Value))
                {
                    Consume(range.End + 1);
                    t.Arguments = range.Value;
                }

                if (!String.IsNullOrEmpty(t.Arguments))
                    AssertExpression("[" + t.Arguments + "]");
            }

            return t;
        }

        private static readonly Regex s_MixinRegex = new Regex(@"\Gmixin +([-\w]+)(?: *\((.*)\))? *");
        private MixinToken Mixin()
        {
            var match = s_MixinRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            var t = CreateToken<MixinToken>(match.Groups[1].Value);
            t.Arguments = match.Groups[2].Value;
            return t;
        }

        private static readonly Regex s_ConditionalRegex = new Regex(@"\G(if|unless|else if|else)\b([^\n]*)");
        private CodeToken Conditional()
        {
            var match = s_ConditionalRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            var js = match.Groups[2].Value;
            var isIf = false;
            var isElse = false;
            switch (match.Groups[1].Value)
            {
                case "if":
                    AssertExpression(js);
                    js = "if (" + js + ")";
                    isIf = true;
                    break;
                case "unless":
                    AssertExpression(js);
                    js = "if (!(" + js + "))";
                    isIf = true;
                    break;
                case "else if":
                    AssertExpression(js);
                    js = "else if (" + js + ")";
                    isIf = true;
                    isElse = true;
                    break;
                case "else":
                    if (!String.IsNullOrWhiteSpace(js))
                        throw new JadeiteParserException(_lineNumber, "`else` cannot have a condition, perhaps you meant `else if`");
                    js = "else";
                    isElse = true;
                    break;
            }

            var t = CreateToken<CodeToken>();
            t.IsIf = isIf;
            t.IsElse = isElse;
            t.RequiresBlock = true;
            return t;
        }

        private static readonly Regex s_WhileRegex = new Regex(@"\Gwhile +([^\n]+)");
        private CodeToken While()
        {
            var match = s_WhileRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            var js = match.Groups[1].Value;
            AssertExpression(js);
            var t = CreateToken<CodeToken>("while (" + js + ")");
            t.RequiresBlock = true;
            return t;
        }

        private static readonly Regex s_EachRegex = new Regex(@"\G(?:- *)?(?:each|for) +([a-zA-Z_$][\w$]*)(?: *, *([a-zA-Z_$][\w$]*))? * in *([^\n]+)");
        private EachToken Each()
        {
            var match = s_EachRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            var t = CreateToken<EachToken>(match.Groups[1].Value);
            t.Key = match.Groups[2].Value;
            if (String.IsNullOrEmpty(t.Key))
                t.Key = "$index";

            var js = match.Groups[3].Value;
            AssertExpression(js);
            t.Code = js;

            return t;
        }

        private static readonly Regex s_CodeRegex = new Regex(@"\G(!?=|-)[ \t]*([^\n]+)");

        private CodeToken Code()
        {
            var match = s_CodeRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            var flags = match.Groups[1].Value;
            var t = CreateToken<CodeToken>(match.Groups[2].Value);
            t.Escape = flags.Length > 0 && flags[0] == '=';
            t.Buffer = t.Escape || (flags.Length > 1 && flags[1] == '=');

            if (t.Buffer)
                AssertExpression(t.Value);

            return t;
        }

        private static readonly Regex s_StartAndEndQuotesRegex = new Regex(@"\G['""]|['""]$");
        private static readonly char[] s_KeyCharList = { ' ', ',', '!', '=', '\n' };
        private AttributesToken Attributes()
        {
            if (CurrentChar != '(')
                return null;

            var range = BracketExpression();
            var t = CreateToken<AttributesToken>();

            AssertNestingCorrect(range.Value);

            Consume(range.End - _inputPosition + 1);

            var quote = '\0';
            var str = range.Value;
            var escapedAttr = true;
            var key = "";
            var val = "";
            var interpolatable = "";
            var state = new CharacterParserState();
            var loc = "key";

            Func<int, bool> isEndOfAttribute = (i) =>
            {
                if (key.Trim() == "")
                    return false;

                if (i == str.Length)
                    return true;

                if (loc == "key")
                {
                    if (str[i] == ' ' || str[i] == '\n')
                    {
                        for (var x = i; x < str.Length; x++)
                        {
                            if (str[x] != ' ' && str[x] != '\n')
                            {
                                if (str[x] == '=' || str[x] == '!' || str[x] == ',')
                                    return false;

                                return true;
                            }
                        }
                    }

                    return str[i] == ',';
                }

                if (loc == "value" && !state.IsNesting)
                {
                    try
                    {
                        AssertExpression(val);
                        if (str[i] == ' ' || str[i] == '\n')
                        {
                            for (var x = i; x < str.Length; x++)
                            {
                                if (str[x] != ' ' && str[x] != '\n')
                                {
                                    if (CharacterParser.IsPunctuator(str[x]) && str[x] != '"' && str[x] != '\'')
                                        return false;

                                    return true;
                                }
                            }
                        }

                        return str[i] == ',';
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                return false;
            }; // end isEndOfAttribute func

            _lineNumber += str.Split('\n').Length - 1;

            for (var i = 0; i <= str.Length; i++)
            {
                if (isEndOfAttribute(i))
                {
                    val = val.Trim();
                    if (val != "")
                        AssertExpression(val);

                    key = key.Trim();
                    key = s_StartAndEndQuotesRegex.Replace(key, "");
                    t.Attributes.Add(new AttributeItem { Name = key, Value = val == "" ? "true" : val, Escaped = escapedAttr });

                    key = "";
                    val = "";
                    loc = "key";
                    escapedAttr = false;
                }
                else
                {
                    switch (loc)
                    {
                        case "key-char":
                            if (str[i] == quote)
                            {
                                loc = "key";
                                if (i + 1 < str.Length && !s_KeyCharList.Contains(str[i + 1]))
                                    throw new JadeiteParserException(_lineNumber, "Unexpected character " + str[i + 1] + " expected ` `, `\\n`, `,`, `!` or `=`");
                            }
                            else
                            {
                                key += str[i];
                            }
                            break;
                        case "key":
                            if (key == "" && (str[i] == '"' || str[i] == '\''))
                            {
                                loc = "key-char";
                                quote = str[i];
                            }
                            else if (str[i] == '!' || str[i] == '=')
                            {
                                escapedAttr = str[i] != '!';
                                if (str[i] == '!')
                                    i++;

                                if (str[i] != '=')
                                    throw new JadeiteParserException(_lineNumber, "Unexpected character " + str[i] + " expected `=`");

                                loc = "value";
                                state = new CharacterParserState();
                            }
                            else
                            {
                                key += str[i];
                            }
                            break;
                        case "value":
                            state.ParseCharacter(str[i]);
                            if (state.IsString)
                            {
                                loc = "string";
                                quote = str[i];
                                interpolatable = str[i].ToString();
                            }
                            else
                            {
                                val += str[i];
                            }
                            break;
                        case "string":
                            state.ParseCharacter(str[i]);
                            interpolatable += str[i];
                            if (!state.IsString)
                            {
                                loc = "value";
                                val += InterpolateAttribute(interpolatable, quote);
                            }
                            break;
                    }
                }
            }

            if (CurrentChar == '/')
            {
                Consume(1);
                t.SelfClosing = true;
            }

            return t;
        }

        private static readonly Regex s_InterpolateAttributeRegex = new Regex(@"(\\)?#\{(.+)");
        private string InterpolateAttribute(string attribute, char quote)
        {
            return s_InterpolateAttributeRegex.Replace(attribute, match =>
            {
                if (match.Groups[1].Success) // escaped
                    return match.Value;

                var expression = match.Groups[2].Value;
                var endIndex = CharacterParser.FindNextUnmatchedBracket(expression);
                if (endIndex != -1 && expression[endIndex] == '}')
                {
                    var insideBrackets = expression.Substring(0, endIndex);
                    AssertExpression(insideBrackets);
                    return quote + " + (" + insideBrackets + ") + " + quote + InterpolateAttribute(expression.Substring(endIndex + 1), quote);
                }

                return match.Groups[1].Value + InterpolateAttribute(expression, quote);
            });
        }

        private static readonly Regex s_AttributesBlockRegex = new Regex(@"\G&attributes\b");
        private AttributesBlockToken AttributesBlock()
        {
            var match = s_AttributesBlockRegex.Match(_input, _inputPosition);
            if (!match.Success)
                return null;

            Consume(match.Length);

            var range = BracketExpression();
            Consume(range.End + 1);
            return CreateToken<AttributesBlockToken>(range.Value);
        }

        private Token Indent()
        {
            var match = GetIndentationMatch();
            if (!match.Success)
                return null;

            Token t;
            var indents = match.Groups[1].Length;

            _lineNumber++;
            Consume(indents + 1);

            if (CurrentChar == ' ' || CurrentChar == '\t')
                throw new JadeiteParserException(_lineNumber, "Invalid indentation, you can use tabs or spaces but not both.");

            // blank line
            if (CurrentChar == '\n')
            {
                _pipeless = false;
                return CreateToken<NewLineToken>();
            }

            if (_indentStack.Count > 0 && indents < _indentStack.Peek()) // outdent
            {
                do
                {
                    _stash.Add(CreateToken<OutdentToken>());
                    _indentStack.Pop();
                }
                while (_indentStack.Count > 0 && indents < _indentStack.Peek());

                t = _stash[_stash.Count - 1];
                _stash.RemoveAt(_stash.Count - 1);
            }
            else if (indents > 0 && (_indentStack.Count == 0 || indents != _indentStack.Peek())) // indent
            {
                _indentStack.Push(indents);
                t = CreateToken<IndentToken>();
                ((IndentToken)t).Indents = indents;
            }
            else // newline
            {
                t = CreateToken<NewLineToken>();
            }

            _pipeless = false;
            return t;
        }

        private PipelessTextToken PipelessText()
        {
            if (!_pipeless)
                return null;

            var match = GetIndentationMatch();
            var indents = match.Success ? match.Groups[1].Length : 0;
            if (indents > 0 && (_indentStack.Count == 0 || indents > _indentStack.Peek()))
            {
                var indent = match.Groups[1].Value;
                var lines = new List<string>();
                bool isMatch;
                do
                {
                    // text has `\n` as a prefix
                    var i = _input.IndexOf("\n", _inputPosition);
                    if (i == -1)
                        i = _input.Length - 1;

                    var str = _input.Substring(_inputPosition + 1, i - _inputPosition);
                    isMatch = str.StartsWith(indent) || str.Trim() == "";
                    if (isMatch)
                    {
                        Consume(str.Length + 1);
                        lines.Add(str.Substring(indent.Length));
                    }
                }
                while (isMatch && _inputPosition < _input.Length);

                while (_inputPosition == _input.Length && lines.Last() == "")
                    lines.RemoveAt(lines.Count - 1);

                var t = CreateToken<PipelessTextToken>();
                t.Lines = lines;
                return t;
            }

            return null;
        }

        private static readonly Regex s_ColonRegex = new Regex(@"\G: *");
        private static readonly Regex s_GoodColonRegex = new Regex(@"\G: +");
        private ColonToken Colon()
        {
            var match = s_GoodColonRegex.Match(_input, _inputPosition);
            var t = Scan<ColonToken>(s_ColonRegex, false);
            if (t != null && !match.Success)
                Debug.WriteLine("Warning: space required after `:` on line {0} of jade file \"{1}\"", _lineNumber, _filename);

            return t;
        }

        private Token Fail()
        {
            throw new JadeiteParserException(_lineNumber, "Unexpected text " + _input.Substring(_inputPosition, 5));
        }

        public Token Advance()
        {
            return Stashed() ?? Next();
        }

        private static readonly Regex s_TabsRegex = new Regex(@"\G\n(\t*) *");
        private static readonly Regex s_SpacesRegex = new Regex(@"\G\n( *)");
        private Match GetIndentationMatch()
        {
            if (_indentRegex != null) // established regexp
            {
                return _indentRegex.Match(_input, _inputPosition);
            }

            // determine regexp
            // check for tabs
            var reg = s_TabsRegex;
            var match = reg.Match(_input, _inputPosition);

            if (match.Success && match.Groups[1].Length == 0)
            {
                // didn't see any tabs, check for spaces instead
                reg = s_SpacesRegex;
                match = reg.Match(_input, _inputPosition);
            }

            if (match.Success && match.Groups[1].Length > 0)
            {
                // established the indentation pattern
                _indentRegex = reg;
            }

            return match;
        }

        private Token Next()
        {
            return Deferred()
                ?? Blank()
                ?? EndOfSource()
                ?? PipelessText()
                ?? Yield()
                ?? DocType()
                ?? Interpolation()
                ?? Case()
                ?? When()
                ?? Default()
                ?? Extends()
                ?? Append()
                ?? Prepend()
                ?? Block()
                ?? MixinBlock()
                ?? Include()
                ?? IncludeFiltered()
                ?? Mixin()
                ?? Call()
                ?? Conditional()
                ?? Each()
                ?? While()
                ?? Tag()
                ?? Filter()
                ?? Code()
                ?? Id()
                ?? Class()
                ?? Attributes()
                ?? AttributesBlock()
                ?? Indent()
                ?? Text()
                ?? Comment()
                ?? Colon()
                ?? Dot()
                ?? TextFail()
                ?? Fail();
        }
    }
}
