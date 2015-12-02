using System;
using System.Collections.Generic;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private readonly Stack<bool> _isBodyInterpolationStack = new Stack<bool>();

        private void TransitionToBody(bool isInterpolation)
        {
            PushState(LexerState.Body);
            _isBodyInterpolationStack.Push(isInterpolation);
        }

        private void OnExitBody()
        {
            _isBodyInterpolationStack.Pop();
        }

        private void ScanBody()
        {
            throw new NotImplementedException();
        }
    }
}
