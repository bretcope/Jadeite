using System;
using System.Diagnostics;

namespace Jadeite.Parsing
{
    public static class ParsingDebug
    {
        [Conditional("DEBUG")]
        public static void AssertKindIsOneOf(JadeiteSyntaxKind kind, params JadeiteSyntaxKind[] oneOf)
        {
            foreach (var k in oneOf)
            {
                if (kind == k)
                    return;
            }

            throw new Exception($"Got kind {kind}. Expected one of: {string.Join(", ", oneOf)}");
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }
    }
}