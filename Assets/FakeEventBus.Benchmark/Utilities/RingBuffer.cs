using System;

namespace FakeEventBus.Benchmark.Utilities
{
    internal class RingBuffer<T>
    {
        private int m_Offset;
        private readonly T[] m_Array;
        
        public int Capacity => m_Array.Length;
        
        public int Length { get; private set; }
        
        public T this[int i] => m_Array[Circle(m_Offset - i, Length)];

        public RingBuffer(int capacity)
        {
            ValidateCapacity(capacity);
            m_Array = new T[capacity];
            m_Offset = -1;
        }

        public void Push(T element)
        {
            m_Offset = (m_Offset + 1) % Capacity;
            m_Array[m_Offset] = element;
            Length = Math.Min(Length + 1, Capacity);
        }

        private static int Circle(int number, int rangeExclusive)
        {
            var result = number % rangeExclusive;

            if ((result < 0 && rangeExclusive > 0) || (result > 0 && rangeExclusive < 0))
            {
                result += rangeExclusive;
            }

            return result;
        }

        private static void ValidateCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    capacity,
                    "Capacity should not be less or equal to zero.");
            }
        }
    }
}
