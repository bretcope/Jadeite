
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jadeite.Parser.Nodes;

namespace Jadeite.Parser
{
    public class ParserOptions
    {
        //
    }

    public class Parser
    {
        private readonly string _input;
        private readonly string _filename;
        private readonly Lexer _lexer;
        private readonly ParserOptions _options;
        private readonly Dictionary<string, BlockNode> _blocks = new Dictionary<string, BlockNode>(); 
        private readonly Dictionary<string, MixinNode> _mixins = new Dictionary<string, MixinNode>();
        private readonly Stack<Parser> _contexts = new Stack<Parser>();
        private int _inMixin = 0;
        //Dependencies
        private int _inBlock = 0;
        private Parser _extendingParser;

        private bool IsIncluded { get; set; }

        public Parser(string str, string filename, ParserOptions options)
        {
            _input = str;
            _filename = filename;
            _lexer = new Lexer(_input, _filename);
            _options = options ?? new ParserOptions();
        }

        private Token Advance()
        {
            return _lexer.Advance();
        }

        private Token Peek()
        {
            return LookAhead(1);
        }

        private Type PeekType()
        {
            return Peek().GetType();
        }

        private int Line()
        {
            return _lexer.LineNumber;
        }

        private Token LookAhead(int n)
        {
            return _lexer.LookAhead(n);
        }

        public BlockNode Parse()
        {
            var block = new BlockNode();
            block.LineNumber = 0;
            block.FileName = _filename;

            while (true)
            {
                var next = Peek();
                if (next.GetType() == TokenTypes.EndOfStream)
                    break;

                if (next.GetType() == TokenTypes.NewLine)
                {
                    Advance();
                    continue;
                }

                var expr = ParseExpression();
                expr.FileName = expr.FileName ?? _filename;
                expr.LineNumber = next.LineNumber;
                block.PushNode(expr);
            }

            if (_extendingParser != null)
            {
                var parser = _extendingParser;
                _contexts.Push(parser);
                var ast = parser.Parse();
                _contexts.Pop();

                // hoist mixins
                foreach (var mixin in _mixins.Values)
                {
                    ast.UnshiftNode(mixin);
                }

                return ast;
            }

            if (_extendingParser == null && !IsIncluded)
            {
                var blocks = new List<string>();
                Utils.WalkAST(block, node =>
                {
                    var b = node as BlockNode;
                    if (b != null && !String.IsNullOrEmpty(b.Name))
                        blocks.Add(b.Name);
                });

                foreach (var b in _blocks.Values)
                {
                    if (!blocks.Contains(b.Name) && !b.IsSubBlock)
                    {
                        throw new JadeiteParserException(b.LineNumber, String.Format("Unexpected block \"{0}\" on line {1} of {2}.", b.Name, b.LineNumber, b.FileName));
                    }
                }
            }

            return block;
        }

        public T Expect<T>() where T : Token
        {
            if (PeekType() == typeof(T))
            {
                return (T)Advance();
            }

            throw new JadeiteParserException(String.Format("Expected '{0}', but got '{1}'.", typeof(T).Name, PeekType().Name));
        }

        public T Accept<T>() where T : Token
        {
            if (PeekType() == typeof(T))
                return (T)Advance();

            return null;
        }

        private Node ParseExpression()
        {
            switch (Peek().JadeEquivalentType)
            {
                case "tag":
                    return ParseTag();
                case "mixin":
                    return ParseMixin();
                case "block":
                    return ParseBlock();
                case "mixin-block":
                    return ParseMixinBlock();
                case "case":
                    return ParseCase();
                case "extends":
                    return ParseExtends();
                case "include":
                    return ParseInclude();
                case "doctype":
                    return ParseDocType();
                case "filter":
                    return ParseFilter();
                case "comment":
                    return ParseComment();
                case "text":
                    return ParseText();
                case "each":
                    return ParseEach();
                case "code":
                    return ParseCode();
                case "call":
                    return ParseCall();
                case "interpolation":
                    return ParseInterpolation();
                case "yield":
                    Advance();
                    var block = new BlockNode();
                    block.IsYield = true;
                    return block;
                case "id":
                case "class":
                    var tok = Advance();
                    _lexer.DeferDiv();
                    _lexer.Defer(tok);
                    return ParseExpression();
                default:
                    throw new Exception("Unexpected token: " + Peek().JadeEquivalentType);
            }
        }

        private Node ParseText()
        {
            var tok = Expect<TextToken>();
            var nodes = ParseInlineTagsInText(tok.Value);
            if (nodes.Count == 1)
                return nodes[0];

            var node = new BlockNode();
            foreach (var n in nodes)
            {
                node.PushNode(n);
            }

            return node;
        }

        private Node ParseBlockExpansion()
        {
            if (PeekType() == typeof(ColonToken))
            {
                Advance();
                return new BlockNode(ParseExpression());
            }

            return Block();
        }

        private Node ParseCase()
        {
            throw new NotImplementedException();
        }

        private Node ParseWhen()
        {
            throw new NotImplementedException();
        }

        private Node ParseDefault()
        {
            throw new NotImplementedException();
        }

        private Node ParseCode()
        {
            throw new NotImplementedException();
        }

        private Node ParseComment()
        {
            throw new NotImplementedException();
        }

        private Node ParseDocType()
        {
            throw new NotImplementedException();
        }

        private Node ParseFilter()
        {
            throw new NotImplementedException();
        }

        private Node ParseEach()
        {
            throw new NotImplementedException();
        }

        // ResolvePath

        private Node ParseExtends()
        {
            throw new NotImplementedException();
        }

        private Node ParseBlock()
        {
            throw new NotImplementedException();
        }

        private Node ParseMixinBlock()
        {
            throw new NotImplementedException();
        }

        private Node ParseInclude()
        {
            throw new NotImplementedException();
        }

        private Node ParseCall()
        {
            throw new NotImplementedException();
        }

        private Node ParseMixin()
        {
            throw new NotImplementedException();
        }

        private static readonly Regex s_InlineTagsRegex = new Regex(@"(\\)?#\[((?:.|\n)*)$");
        private IList<Node> ParseInlineTagsInText(string str)
        {
            var line = Line();

            var list = new List<Node>();
            TextNode text;
            var match = s_InlineTagsRegex.Match(str);
            if (match.Success)
            {
                if (match.Groups[1].Success) // escaped
                {
                    text = new TextNode(str.Substring(0, match.Index) + "#[");
                    text.LineNumber = line;
                    var rest = ParseInlineTagsInText(match.Groups[2].Value);
                    if (rest[0] is TextNode)
                    {
                        text.Value += ((TextNode)rest[0]).Value;
                        rest.RemoveAt(0);
                    }
                    
                    list.Add(text);
                    list.AddRange(rest);
                }
                else
                {
                    text = new TextNode(str.Substring(0, match.Index));
                    text.LineNumber = line;
                    list.Add(text);
                    var rest = match.Groups[2].Value;
                    var bracketDex = CharacterParser.FindNextUnmatchedBracket(rest);
                    if (bracketDex == -1)
                        throw new JadeiteParserException("The end of the string was reached with no closing bracket found.");
                    var inner = new Parser(rest.Substring(0, bracketDex), _filename, _options);
                    list.Add(inner.Parse());
                    list.AddRange(ParseInlineTagsInText(rest.Substring(bracketDex + 1)));
                }
            }
            else
            {
                text = new TextNode(str);
                text.LineNumber = line;
                list.Add(text);
            }

            return list;
        }

        private Node ParseTextBlock()
        {
            throw new NotImplementedException();
        }

        private Node Block()
        {
            throw new NotImplementedException();
        }

        private Node ParseInterpolation()
        {
            throw new NotImplementedException();
        }

        private Node ParseTag()
        {
            throw new NotImplementedException();
        }

        private Node Tag()
        {
            throw new NotImplementedException();
        }
    }
}
