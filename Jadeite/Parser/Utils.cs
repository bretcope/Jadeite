
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Jadeite.Parser.Nodes;

namespace Jadeite.Parser
{
    public delegate void AstDelegate(Node ast);

    public static class Utils
    {
        public static void WalkAST(Node ast, AstDelegate before = null, AstDelegate after = null)
        {
            if (before != null)
                before(ast);

            switch (ast.Type)
            {
                case "Block":
                    foreach (var node in ((BlockNode)ast).Nodes)
                    {
                        WalkAST(node, before, after);
                    }
                    break;
                case "Case":
                case "Each":
                case "Mixin":
                case "Tag":
                case "When":
                case "Code":
                    if (ast.Block != null)
                        WalkAST(ast.Block, before, after);
                    break;
                case "Attrs":
                case "BlockComment":
                case "Comment":
                case "Doctype":
                case "Filter":
                case "Literal":
                case "MixinBlock":
                case "Text":
                    break;
                default:
                    throw new Exception("Unexpected node type " + ast.Type);
            }

            if (after != null)
                after(ast);
        }

        public static string PathJoin(params string[] parts)
        {
            return PathNormalize(String.Join(Path.DirectorySeparatorChar.ToString(), parts));
        }

        public static string PathNormalize(string path)
        {
            var parts = PathGetNormalizedParts(path);
            return String.Join(Path.DirectorySeparatorChar.ToString(), parts);
        }

        public static List<string> PathGetNormalizedParts(string path)
        {
            var parts = PathSplit(path).ToList();

            // normalize parts
            for (var i = 0; i < parts.Count;)
            {
                var p = parts[i];
                switch (p)
                {
                    case "":
                    case ".":
                        if (i != 0)
                        {
                            parts.RemoveAt(i);
                            continue;
                        }
                        break;
                    case "..":
                        if (i > 0 && parts[i - 1] != ".." && parts[i - 1] != "" && !IsDriveLetter(parts[i - 1]))
                        {
                            i--;
                            parts.RemoveRange(i, 2);
                            continue;
                        }
                        break;
                }

                i++;
            }

            return parts;
        }

        private static readonly Regex s_DriveLetterRegex = new Regex(@"[a-zA-Z]:");
        public static bool IsDriveLetter(string part)
        {
            return s_DriveLetterRegex.IsMatch(part);
        }

        public static string PathBaseName(string path, string ext = null)
        {
            var parts = PathSplit(path);
            var name = "";
            for (var i = parts.Length - 1; i > -1; i--)
            {
                if (parts[i] == "")
                    continue;

                name = parts[i];
                if (!String.IsNullOrEmpty(ext) && name.EndsWith(ext))
                    name = name.Substring(0, name.Length - ext.Length);

                break;
            }

            return name;
        }

        public static string PathDirName(string path)
        {
            var parts = PathGetNormalizedParts(path);
            if (parts.Count > 0)
            {
                parts.RemoveAt(parts.Count - 1);
            }

            return String.Join(Path.DirectorySeparatorChar.ToString(), parts);
        }

        private static readonly Regex s_PathDelimiterRegex = new Regex(@"[/\\]+");
        public static string[] PathSplit(string path)
        {
            return s_PathDelimiterRegex.Split(path);
        }
    }
}
