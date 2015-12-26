using System;

namespace Jadeite.Parsing
{
    public class LookAheadQueue<T>
    {
        private T[] _array;
        private int _head; // index of the oldest item in the array
        private int _tail; // index where the next item will be written

        public int Count { get; private set; }

        public LookAheadQueue(int capacity = 8)
        {
            if (capacity <= 0)
                throw new Exception("DoubleEndedQueue initial capacity must be at least 1.");

            _array = new T[capacity];
        }

        public void Push(T item)
        {
            if (Count == _array.Length)
                Grow();

            _array[_tail] = item;

            Count++;
            _tail++;
            if (_tail >= _array.Length)
                _tail = 0;
        }

        public T Current()
        {
            if (Count == 0)
                throw new Exception("Cannot read the current item. Queue is empty.");

            return _array[_head];
        }

        public T LookAhead()
        {
            if (Count < 2)
                throw new Exception("Cannot perform look-ahead when there aren't at least two items in the queue.");

            var index = _head - 1;
            if (index < 0)
                index = _array.Length - 1;

            return _array[index];
        }

        public T Dequeue()
        {
            if (Count == 0)
                throw new Exception("Cannot dequeue. Queue is empty.");


            var item = _array[_head];
            _array[_head] = default(T);

            Count--;
            _head++;
            if (_head >= _array.Length)
                _head = 0;

            return item;
        }

        private void Grow()
        {
            var oldArray = _array;
            var newArray = new T[_array.Length * 2];

            if (_head == 0)
            {
                // easy optimization in the event head and tail are perfectly aligned with the array
                Array.Copy(oldArray, newArray, Count);
            }
            else
            {
                var frontLength = oldArray.Length - _head;
                Array.Copy(oldArray, _head, newArray, 0, frontLength);
                Array.Copy(oldArray, 0, newArray, frontLength, Count - frontLength);
            }

            _array = newArray;

            _head = 0;
            _tail = Count;
        }
    }
}
