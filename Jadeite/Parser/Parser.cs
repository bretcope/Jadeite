
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Jadeite.Parser.Nodes;

namespace Jadeite.Parser
{
    public class ParserOptions
    {
        public string BaseDir { get; set; }
    }

    public class Parser
    {
        private readonly string _input;
        private readonly string _filename;
        private readonly Lexer _lexer;
        private readonly ParserOptions _options;
        private Dictionary<string, BlockNode> _blocks = new Dictionary<string, BlockNode>(); 
        private readonly Dictionary<string, MixinNode> _mixins = new Dictionary<string, MixinNode>();
        private Stack<Parser> _contexts = new Stack<Parser>();
        private int _inMixin = 0;
        private List<string> _dependencies = new List<string>();
        private int _inBlock = 0;
        private Parser _extendingParser;

        private bool IsIncluded { get; set; }

        public IReadOnlyList<string> Dependencies => _dependencies.AsReadOnly();

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
            if (PeekType() == TokenTypes.Colon)
            {
                Advance();
                return new BlockNode(ParseExpression());
            }

            return Block();
        }

        private Node ParseCase()
        {
            var node = new CaseNode(Expect<CaseToken>().Value);
            node.LineNumber = Line();

            var block = new BlockNode();
            block.LineNumber = Line();
            block.FileName = _filename;
            Expect<IndentToken>();
            while(PeekType() != TokenTypes.Outdent)
            {
                if (PeekType() == TokenTypes.Comment || PeekType() == TokenTypes.NewLine)
                {
                    Advance();
                }
                else if (PeekType() == TokenTypes.When)
                {
                    block.PushNode(ParseWhen());
                }
                else if (PeekType() == TokenTypes.Default)
                {
                    block.PushNode(ParseDefault());
                }
                else
                {
                    throw new JadeiteParserException("Unexpected token '" + PeekType().Name + "' expected 'when', 'default', or a new line.");
                }
            }

            Expect<OutdentToken>();

            node.Block = block;
            return node;
        }

        private Node ParseWhen()
        {
            var val = Expect<WhenToken>().Value;
            if (PeekType() != TokenTypes.NewLine)
                return new WhenNode(val, ParseBlockExpansion());

            return new WhenNode(val);
        }

        private Node ParseDefault()
        {
            Expect<DefaultToken>();
            return new WhenNode("default", ParseBlockExpansion());
        }

        private Node ParseCode()
        {
            var tok = Expect<CodeToken>();
            var node = new CodeNode(tok.Value, tok.Buffer, tok.Escape);
            node.LineNumber = Line();

            // throw an error if an else does not have an if
            if (tok.IsElse && !tok.HasIf)
                throw new JadeiteParserException("Unexpected else without if");

            // handle block
            var hasBlock = PeekType() == TokenTypes.Indent;
            if (hasBlock)
                node.Block = Block();

            // handle missing block
            if (tok.RequiresBlock && !hasBlock)
                node.Block = new BlockNode();

            // mark presense of if for future elses
            if (tok.IsIf)
            {
                var code = Peek() as CodeToken;
                if (code == null && PeekType() == TokenTypes.NewLine)
                    code = LookAhead(2) as CodeToken;

                if (code != null && code.IsElse)
                    code.HasIf = true;
            }

            return node;
        }

        private Node ParseComment()
        {
            var tok = Expect<CommentToken>();
            Node node;

            var block = ParseTextBlock();
            if (block != null)
                node = new BlockCommentNode(tok.Value, block, tok.Buffer);
            else
                node = new CommentNode(tok.Value, tok.Buffer);

            node.LineNumber = Line();
            return node;
        }

        private Node ParseDocType()
        {
            var node = new DocTypeNode(Expect<DocTypeToken>().Value);
            node.LineNumber = Line();
            return node;
        }

        private Node ParseFilter()
        {
            var tok = Expect<FilterToken>();
            var attributes = Accept<AttributesToken>();

            var block = ParseTextBlock() ?? new BlockNode();
            var node = new FilterNode(tok.Value, block, attributes?.Attributes);
            node.LineNumber = Line();
            return node;
        }

        private Node ParseEach()
        {
            var tok = Expect<EachToken>();
            var node = new EachNode(tok.Code, tok.Value, tok.Key);
            node.LineNumber = Line();
            node.Block = Block();

            var code = Peek() as CodeToken;
            if (code != null && code.Value == "else")
            {
                Advance();
                node.Alternative = Block();
            }

            return node;
        }

        private string ResolvePath(string path, string purpose)
        {
            if (!path.StartsWith("/") && String.IsNullOrEmpty(_filename))
                throw new JadeiteParserException("The filename option is required to use '" + purpose + "' with relative paths.");

            if (path.StartsWith("/") && String.IsNullOrEmpty(_options.BaseDir))
                throw new JadeiteParserException("The ParserOptions.BaseDir option is required to use '" + purpose + "' with absolute paths.");

            path = Utils.PathJoin(path[0] == '/' ? _options.BaseDir : Utils.PathDirName(_filename), path);

            if (!Utils.PathBaseName(path).EndsWith(".jade"))
                path += ".jade";

            return path;
        }

        private Node ParseExtends()
        {
            var path = ResolvePath(Expect<ExtendsToken>().Value.Trim(), "extends");

            _dependencies.Add(path);
            var str = File.ReadAllText(path, new UTF8Encoding(false));
            var parser = new Parser(str, path, _options);

            parser._dependencies = _dependencies;
            parser._blocks = _blocks;
            parser.IsIncluded = IsIncluded;
            parser._contexts = _contexts;
            parser._extendingParser = _extendingParser;

            // todo: null node
            return new LiteralNode("");
        }

        private BlockNode ParseBlock()
        {
            var tok = Expect<BlockToken>();

            _inBlock++;
            var block = PeekType() == TokenTypes.Indent ? Block() : new BlockNode(new LiteralNode(""));
            _inBlock--;
            block.Name = tok.Value.Trim();
            block.LineNumber = tok.LineNumber;

            IEnumerable<Node> allNodes;
            BlockNode prev = null;
            if (_blocks.TryGetValue(block.Name, out prev))
            {
                if (prev.Mode == BlockMode.Replace)
                    return prev;

                allNodes = prev.Prepended.Concat(block.Nodes).Concat(prev.Appended);
            }
            else
            {
                prev = new BlockNode();
                allNodes = block.Nodes;
            }

            switch (tok.Mode)
            {
                case BlockMode.Append:
                    prev.Appended = prev.Parser == this ?
                        prev.Appended.Concat(block.Nodes).ToList() :
                        block.Nodes.Concat(prev.Appended).ToList();
                    break;
                case BlockMode.Prepend:
                    prev.Prepended = prev.Parser == this ?
                        block.Nodes.Concat(prev.Prepended).ToList() :
                        prev.Prepended.Concat(block.Nodes).ToList();
                    break;
            }

            block.Nodes = allNodes.ToList();
            block.Appended = prev.Appended;
            block.Prepended = prev.Prepended;
            block.Mode = tok.Mode;
            block.Parser = this;

            block.IsSubBlock = _inBlock > 0;

            _blocks[block.Name] = block;
            return block;
        }

        private MixinBlockNode ParseMixinBlock()
        {
            Expect<MixinBlockToken>();
            if (_inMixin == 0)
                throw new JadeiteParserException("Anonymous blocks are not allowed unless they are part of a mixin.");

            return new MixinBlockNode();
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

        private BlockNode ParseTextBlock()
        {
            throw new NotImplementedException();
        }

        private BlockNode Block()
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
