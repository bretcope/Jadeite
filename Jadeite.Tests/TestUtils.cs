using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Jadeite.Parser;

namespace Jadeite.Tests
{
    public static class TestUtils
    {
        public static readonly string JadeiteRoot;
        public static readonly string NodeRoot;
        public static readonly string JadeRoot;
        public static readonly string ExamplesRoot;

        static TestUtils()
        {
            JadeiteRoot = FindSolutionRoot();

            NodeRoot = Path.Combine(JadeiteRoot, "node");
            JadeRoot = Path.Combine(NodeRoot, "jade");
            ExamplesRoot = Path.Combine(JadeRoot, "examples");
        }

        private static string FindSolutionRoot()
        {
            // try to find Jadeite root (assume it's going to be 
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            do
            {
                if (File.Exists(Path.Combine(dir.FullName, "Jadeite.sln")))
                {
                    return dir.FullName;
                }
            }
            while ((dir = dir.Parent) != null);

            throw new Exception("Unable to determine the root of the Jadeite solution.");
        }

        public static string[] GetExampleFiles()
        {
            return Directory.GetFiles(ExamplesRoot, "*.jade");
        }

        public static string GetNodeResult(string script, string args)
        {
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "node",
                    Arguments = Path.Combine(NodeRoot, script) + " " + args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
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
            var jade = File.ReadAllText(filename);

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
            return GetNodeResult("LexerDump.js", "\"" + filename + "\"").Trim();
        }
    }
}