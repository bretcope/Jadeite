using System;
using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(
        JadeiteKind.EscapedBufferedCode,
        JadeiteKind.UnescapedBufferedCode,
        JadeiteKind.InterpolatedEscapedBufferedCode,
        JadeiteKind.InterpolatedUnescapedBufferedCode
    )]
    public sealed class BufferedCodeNode : INode, ICustomDebugNode
    {
        [AssertNotNull]
        public Token BeginningToken { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Expression { get; internal set; }
        [AssertKind(true, JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteKind Kind { get; }

        internal BufferedCodeNode(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return BeginningToken;
            yield return Expression;
            yield return EndOfLine;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            switch (Kind)
            {
                case JadeiteKind.EscapedBufferedCode:
                    ParsingDebug.AssertKindIsOneOf(BeginningToken.Kind, JadeiteKind.Equals);
                    ParsingDebug.Assert(EndOfLine != null);
                    break;
                case JadeiteKind.UnescapedBufferedCode:
                    ParsingDebug.AssertKindIsOneOf(BeginningToken.Kind, JadeiteKind.BangEquals);
                    ParsingDebug.Assert(EndOfLine != null);
                    break;
                case JadeiteKind.InterpolatedEscapedBufferedCode:
                    ParsingDebug.AssertKindIsOneOf(BeginningToken.Kind, JadeiteKind.Equals);
                    ParsingDebug.Assert(EndOfLine == null);
                    break;
                case JadeiteKind.InterpolatedUnescapedBufferedCode:
                    ParsingDebug.AssertKindIsOneOf(BeginningToken.Kind, JadeiteKind.BangEquals);
                    ParsingDebug.Assert(EndOfLine == null);
                    break;
                default:
                    throw new Exception($"{Kind} is not being validated in BufferedCodeNode");
            }
        }
    }
}