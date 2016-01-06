using System.Collections.Generic;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public interface ISyntaxElement
    {
        JadeiteSyntaxKind Kind { get; }
    }

    public interface INode : ISyntaxElement
    {
        IEnumerable<ISyntaxElement> GetChildren();
    }
}