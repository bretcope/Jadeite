using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.SwitchLabel)]
    public sealed class SwitchLabelNode : INode, ICustomDebugNode
    {
        [AssertKind(JadeiteKind.CaseKeyword, JadeiteKind.DefaultKeyword)]
        public Token CaseOrDefault { get; internal set; }
        [AssertKind(true, JadeiteKind.ExpressionList)]
        public ExpressionListNode Expressions { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.SwitchLabel;

        internal SwitchLabelNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return CaseOrDefault;

            if (Expressions != null)
                yield return Expressions;

            yield return EndOfLine;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            if (CaseOrDefault.Kind == JadeiteKind.CaseKeyword)
                ParsingDebug.Assert(Expressions != null);
            else 
                ParsingDebug.Assert(Expressions == null);
        }
    }
}