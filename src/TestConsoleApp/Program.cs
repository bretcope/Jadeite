using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Jadeite.Parser;

namespace TestConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var test = File.ReadAllText("Master.jade");

            var lexer = new Lexer(test, "  ");
            Token tok;
            while ((tok = lexer.Advance()).Kind != JadeiteSyntaxKind.EndOfInput)
            {
                PrintToken(tok);
            }
        }

        public static void PrintToken(Token tok)
        {
            Console.WriteLine(tok.Kind);
            if (tok.Value != null)
                Console.WriteLine($"  `{tok.Value}`");
        }
    }
}
