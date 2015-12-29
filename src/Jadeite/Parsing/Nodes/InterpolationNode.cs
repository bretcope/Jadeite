using System;

namespace Jadeite.Parsing.Nodes
{
    public class InterpolationNode : INode
    { 
        public JadeiteSyntaxKind Kind { get; }
        public ElementList Children { get; } = new ElementList();

        internal InterpolationNode(JadeiteSyntaxKind kind)
        {
#if DEBUG
            switch (kind)
            {
                case JadeiteSyntaxKind.InterpolatedTag:
                case JadeiteSyntaxKind.EscapedInterpolatedExpression:
                case JadeiteSyntaxKind.UnescapedInterpolatedExpression:
                    break;
                default:
                    throw new Exception($"{kind} is not an interpolation kind.");
            }
#endif

            Kind = kind;
        }
    }
}