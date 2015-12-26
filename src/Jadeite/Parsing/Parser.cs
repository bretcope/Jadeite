using System;
using System.Collections.Generic;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public partial class Parser
    {
        private readonly Lexer _lexer;
        private readonly Stack<IParentElement> _parentStack = new Stack<IParentElement>();

        public Parser(string input, string indent)
        {
            _lexer = new Lexer(input, indent);
        }

        public StartNode Parse()
        {
            var start = new StartNode();

            while (_lexer.Current().Kind == JadeiteSyntaxKind.EndOfLine)
            {
                start.AddEndOfLine(_lexer.Advance());
            }

            var file = ParseFile();
            start.SetFile(file);

            // loop

            // check to make sure we're at end of input
            if (_lexer.Current().Kind != JadeiteSyntaxKind.EndOfInput)
                throw new Exception("Parser did not consume all input."); // todo

            return start;
        }

        private FileNode ParseFile()
        {
            //
        }
    }
}