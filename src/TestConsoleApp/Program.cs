using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Jadeite.Internals;

namespace TestConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var test = @"
a.some-class#myId This is body text #[b and bold text], isn't that great?
.test
    p: a Text
    p
        | piped text
";

            var lexer = new Lexer(test, "    ");
            Token tok;
            while ((tok = lexer.Advance()).Type != TokenType.EndOfInput)
            {
                PrintToken(tok);
            }
        }

        public static void PrintToken(Token tok)
        {
            Console.WriteLine(tok.Type);
            Console.WriteLine($"  `{tok.Value}`");
        }
    }
}
