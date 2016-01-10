using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.DoctypeDefinition)]
    public sealed class DoctypeDefinitionNode : INode
    {
        [AssertKind(JadeiteKind.DoctypeKeyword)]
        public Token DoctypeKeyword { get; internal set; }
        [AssertNotNull]
        public TextBodyElementListNode Body { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.DoctypeDefinition;

        internal DoctypeDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return DoctypeKeyword;
            yield return Body;
            yield return EndOfLine;
        }
    }
}