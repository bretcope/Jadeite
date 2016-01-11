using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.SwitchStatement)]
    public sealed class SwitchNode : INode
    {
        [AssertKind(JadeiteKind.SwitchKeyword)]
        public Token SwitchKeyword { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Expression { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(JadeiteKind.SwitchBody)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.SwitchStatement;

        internal SwitchNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return SwitchKeyword;
            yield return Expression;
            yield return EndOfLine;
            yield return Block;
        }
    }
}