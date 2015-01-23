using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Jadeite.Parser;

namespace TestConsole
{
    public class Program
    {
        public void Main(string[] args)
        {
            var node = TestUtils.DumpNodeLexer("extend.jade");
            var cs = TestUtils.DumpCSharpLexer("extend.jade");
            Console.WriteLine(node == cs);
        }
    }
}
