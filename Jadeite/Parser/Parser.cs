
using System;
using System.Collections;
using System.Collections.Generic;
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
        //Blocks
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
                //
            }

            throw new NotImplementedException();
        }

        private Node ParseExpression()
        {
            throw new NotImplementedException();
        }
    }
}
