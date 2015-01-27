
using System;
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
    }
}
