using System;
using Jadeite.Tests;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = "attributes.jade";
            var node = TestUtils.DumpNodeLexer(file);
            var cs = TestUtils.DumpCSharpLexer(file);
            Console.WriteLine(node == cs);
            
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
