using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Jadeite.Parsing.Nodes
{
    public sealed class SyntaxList<T> : IEnumerable<T> where T : ISyntaxElement
    {
        private readonly List<T> _elements = new List<T>();

        public int Count => _elements.Count;
        public T this[int index] => _elements[index];
        
        internal SyntaxList() { }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(T e)
        {
            _elements.Add(e);
        }
    }
}