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

            while (Current.Kind == JadeiteSyntaxKind.EndOfLine)
            {
                start.AddEndOfLine(Advance());
            }

            var file = ParseFile();
            start.SetFile(file);

            // check to make sure we're at end of input
            if (Current.Kind != JadeiteSyntaxKind.EndOfInput)
                throw new Exception("Parser did not consume all input."); // todo

            return start;
        }

        private FileNode ParseFile()
        {
            var file = new FileNode();
            file.SetTemplate(ParseTemplate());

            if (Current.Kind == JadeiteSyntaxKind.MixinKeyword)
                file.SetMixins(ParseMixinList());

            return file;
        }
    }
}