using System.Collections.Generic;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public interface ISyntaxElement
    {
        JadeiteKind Kind { get; }
    }

    public interface INode : ISyntaxElement
    {
        IEnumerable<ISyntaxElement> GetChildren();
    }

    internal interface ICustomDebugNode
    {
        void AssertIsValid();
    }
}