using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jadeite.Parsing;
using Jadeite.Parsing.Nodes;

namespace TestConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
//            var test = File.ReadAllText("Master.jade");
//            var indent = "  ";
            var test = File.ReadAllText(@"Sample\StandAlone.jadeite");
            var indent = "\t";

//            PrintLexer(test, indent);
            PrintParser(test, indent);
        }

        private static void PrintLexer(string input, string indent)
        {
            var lexer = new Lexer(input, indent);
            Token tok;
            while ((tok = lexer.Advance()).Kind != JadeiteKind.EndOfInput)
            {
                PrintToken(tok);
            }
        }

        private static void PrintToken(Token tok)
        {
            Console.WriteLine(tok.Kind);
            if (tok.Value != null)
                Console.WriteLine($"  `{tok.Value}`");
        }

        private static void PrintParser(string input, string indent)
        {
            var parser = new Parser(input, indent);
            var start = parser.Parse();
            PrintSyntaxElement(start, "");
        }

        private static void PrintSyntaxElement(ISyntaxElement element, string indent)
        {
            var value = (element as Token)?.Value?.ToString() ?? "";
            if (value != "")
                value = " `" + value + "`";

            Console.WriteLine(indent + element.Kind + value);

            var node = element as INode;
            if (node != null)
            {
                indent += node is BlockNode ? "| " : "  ";
                foreach (var child in node.GetChildren())
                {
                    PrintSyntaxElement(child, indent);
                }
            }
        }
    }
}
