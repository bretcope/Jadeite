using System;
using System.IO;
using NUnit.Framework;

namespace Jadeite.Tests
{
    [TestFixture]
    class LexerTests
    {
        [Test]
        public void CompareToJavaScriptLexer()
        {
            Exception ex = null;
            foreach (var example in TestUtils.GetExampleFiles())
            {
                var file = TestUtils.GetExampleFiles()[0];
                var node = TestUtils.DumpNodeLexer(file);
                var cs = TestUtils.DumpCSharpLexer(file);

                var name = Path.GetFileName(example);

                if (node == cs)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(name + ": good");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(name + ": Differences");
                    Console.ResetColor();
                    PrintQuasiDiff(cs, node);
                    Console.WriteLine();

                    if (ex == null)
                        ex = new Exception("CS and JS Lexers do not agree on " + name);
                }
            }

            if (ex != null)
                throw ex;
        }

        private static void PrintQuasiDiff(string cs, string node)
        {
            var csLines = cs.Split('\n');
            var nodeLines = node.Split('\n');

            // print a *really* primitive diff - mostly only care about the point where they diverge anyway
            var len = Math.Max(csLines.Length, nodeLines.Length);
            for (var i = 0; i < len; i++)
            {
                if (i < csLines.Length && i < nodeLines.Length)
                {
                    if (csLines[i] == nodeLines[i])
                    {
                        Console.WriteLine(csLines[i]);
                        continue;
                    }
                }

                if (i < csLines.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("cs:" + csLines[i]);
                    Console.ResetColor();
                }

                if (i < nodeLines.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("js:" + nodeLines[i]);
                    Console.ResetColor();
                }
            }
        }
    }
}
