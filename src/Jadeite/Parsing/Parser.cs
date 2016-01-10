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

            if (Current.Kind == JadeiteKind.EndOfLine)
                start.EndOfLines = ParseEndOfLineList();

            start.File = ParseFile();

            // check to make sure we're at end of input
            if (Current.Kind != JadeiteKind.EndOfInput)
                throw new Exception("Parser did not consume all input."); // todo

            return start;
        }

        private EndOfLineListNode ParseEndOfLineList()
        {
            AssertCurrentKind(JadeiteKind.EndOfLine);

            var endOfLines = new EndOfLineListNode();

            do
            {
                endOfLines.Add(Advance());

            } while (Current.Kind == JadeiteKind.EndOfLine);

            return endOfLines;
        }

        private FileNode ParseFile()
        {
            var file = new FileNode();
            file.Template = ParseTemplate();

            if (Current.Kind == JadeiteKind.MixinKeyword)
                file.Mixins = ParseMixinList();

            return file;
        }
    }
}