using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DoctypeDefinitionNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.DoctypeKeyword)]
        public Token DoctypeKeyword { get; internal set; }
        [AssertNotNull]
        public TextBodyElementListNode Body { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DoctypeDefinition;

        internal DoctypeDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return DoctypeKeyword;
            yield return Body;
            yield return EndOfLine;
        }
    }
}