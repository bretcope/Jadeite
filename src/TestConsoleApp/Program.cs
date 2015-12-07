using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Jadeite.Internals;

namespace TestConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var test = File.ReadAllText("Master.jade");

            var lexer = new Lexer(test, "  ");
            Token tok;
            while ((tok = lexer.Advance()).Type != TokenType.EndOfInput)
            {
                PrintToken(tok);
            }
        }

        public static void PrintToken(Token tok)
        {
            Console.WriteLine(tok.Type);
            if (tok.Value != null)
                Console.WriteLine($"  `{tok.Value}`");
        }
    }
}
