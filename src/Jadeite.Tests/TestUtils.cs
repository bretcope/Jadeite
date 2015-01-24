using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Jadeite.Parser;

namespace Jadeite.Tests
{
    public static class TestUtils
    {
        public static readonly string NodeRoot = "../../node/";
        public static readonly string JadeRoot = NodeRoot + "jade/";
        public static readonly string ExamplesRoot = JadeRoot + "examples/";

        static TestUtils()
        {
            NodeRoot = NodeRoot.Replace('/', Path.DirectorySeparatorChar);
            JadeRoot = JadeRoot.Replace('/', Path.DirectorySeparatorChar);
            ExamplesRoot = ExamplesRoot.Replace('/', Path.DirectorySeparatorChar);
        }

        public static string GetExample(string filename)
        {
            filename = filename.Replace('/', Path.PathSeparator);
            return File.ReadAllText(ExamplesRoot + filename);
        }

        public static string GetNodeResult(string script, string args)
        {
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "node",
                    Arguments = NodeRoot + script + " " + args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            p.Start();

            var stdout = p.StandardOutput.ReadToEnd();
            var stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            if (p.ExitCode != 0)
                throw new Exception(stderr);

            return stdout;
        }

        private static string Escape(string str)
        {
            return str.Replace("\"", "\\\"");
        }

        public static string DumpCSharpLexer(string filename)
        {
            var jade = GetExample(filename);

            var lexer = new Lexer(jade, filename);
            Token t;
            int i = 0;
            StringBuilder dump = new StringBuilder();
            try
            {
                while (true)
                {
                    t = lexer.Advance();
                    if (t.GetType() == TokenTypes.EndOfStream)
                        break;

                    dump.AppendFormat("{0}: {1}\n", i, t.JadeEquivalentType);

                    if (t.JadeEquivalentType == "indent")
                    {
                        dump.AppendFormat("    Indents: {0}\n", ((IndentToken) t).Indents);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(t.Value))
                        {
                            dump.AppendFormat("    Value: \"{0}\"\n", Escape(t.Value));
                        }

                        if (t.JadeEquivalentType == "attrs")
                        {
                            var attrs = (AttributesToken) t;
                            dump.AppendFormat("    Attributes: {0}\n", attrs.Attributes.Count);
                            foreach (var a in attrs.Attributes)
                            {
                                dump.AppendFormat("        Name: \"{0}\", Value: \"{1}\", Escaped: {2}\n",
                                    Escape(a.Name), Escape(a.Value), a.Escaped ? "true" : "false");
                            }
                        }
                    }

                    i++;
                }
            }
            catch (Exception)
            {
                Console.WriteLine(dump.ToString());
                throw;
            }

            return dump.ToString().Trim();
        }

        public static string DumpNodeLexer(string filename)
        {
            return GetNodeResult("LexerDump.js", filename).Trim();
        }
    }
}