using System;
using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.DocumentBlock, JadeiteKind.PipelessTextBlock, JadeiteKind.SwitchBody)]
    public sealed class BlockNode : INode, ICustomDebugNode
    {
        [AssertKind(JadeiteKind.Indent)]
        public Token Indent { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Body { get; internal set; }
        [AssertKind(JadeiteKind.Outdent)]
        public Token Outdent { get; internal set; }

        public JadeiteKind Kind { get; }

        internal BlockNode(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Indent;
            yield return Body;
            yield return Outdent;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            if (Kind == JadeiteKind.DocumentBlock && Body.Kind == JadeiteKind.DocumentBody)
                return;

            if (Kind == JadeiteKind.PipelessTextBlock && Body.Kind == JadeiteKind.TextBodyElementList)
                return;

            if (Kind == JadeiteKind.SwitchBody && Body.Kind == JadeiteKind.SwitchSectionList)
                return;

            throw new Exception($"Incorrect body kind for {Kind}.");
        }
    }
}