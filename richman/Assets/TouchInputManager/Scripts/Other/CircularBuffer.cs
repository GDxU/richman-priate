using System;
using System.Collections;
using System.Collections.Generic;

namespace PigeonCoopUtil
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private T[] _buffer;

        private int _latestIndex = -1;
        private bool bufferFull = false;

        public int bufferSize { get; private set; }

        public int Count
        {
            get
            {
                if (bufferFull)
                    return bufferSize;
                else
                    return _latestIndex + 1;
            }
        }

        public CircularBuffer(int size)
        {
            bufferSize = size;
            _latestIndex = -1;
            _buffer = new T[bufferSize];
        }

        public void Add(T item)
        {
            _latestIndex++;

            if (_latestIndex == bufferSize)
            {
                bufferFull = true;
                _latestIndex = 0;
            }

            _buffer[_latestIndex] = item;
        }

        public void Clear()
        {
            _buffer = new T[bufferSize];
            _latestIndex = -1;
        }

        public T First()
        {
            if (_latestIndex < 0)
                return default(T);
            else
                return _buffer[_latestIndex];
        }


        public IEnumerator<T> GetEnumerator()
        {
            if (_latestIndex < 0)
                yield break;

            int currentIndex = _latestIndex;
            int loopCounter = 0;
            while (loopCounter != bufferSize)
            {
                loopCounter++;
                yield return _buffer[currentIndex];

                currentIndex--;
                if (currentIndex < 0)
                {
                    if (bufferFull)
                        currentIndex = bufferSize - 1;
                    else
                        yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}