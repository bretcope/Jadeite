using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public interface ISyntaxElement
    {
        JadeiteSyntaxKind Kind { get; }
    }

    public interface IParentElement : ISyntaxElement
    {
        ElementList Body { get; }
    }
}