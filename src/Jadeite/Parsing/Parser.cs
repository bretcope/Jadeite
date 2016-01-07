using System;
using System.Collections.Generic;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public sealed partial class Parser : ParserBase
    {
        public Parser(string input, string indent) : base(input, indent)
        {
        }

        public StartNode Parse()
        {
            var start = new StartNode();

            if (Current.Kind == JadeiteSyntaxKind.EndOfLine)
                start.EndOfLines = ParseEndOfLineList();

            start.File = ParseFile();

            // check to make sure we're at end of input
            if (Current.Kind != JadeiteSyntaxKind.EndOfInput)
                throw new Exception("Parser did not consume all input."); // todo

            return start;
        }

        private EndOfLineListNode ParseEndOfLineList()
        {
            AssertCurrentKind(JadeiteSyntaxKind.EndOfLine);

            var endOfLines = new EndOfLineListNode();

            do
            {
                endOfLines.Add(Advance());

            } while (Current.Kind == JadeiteSyntaxKind.EndOfLine);

            return endOfLines;
        }

        private FileNode ParseFile()
        {
            var file = new FileNode();
            file.Template = ParseTemplate();

            if (Current.Kind == JadeiteSyntaxKind.MixinKeyword)
                file.Mixins = ParseMixinList();

            return file;
        }
    }
}