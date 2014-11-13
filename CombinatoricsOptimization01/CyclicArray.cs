using System;

namespace CombinatoricsOptimization01
{
    public class CyclicArray<T>
    {
        int length;
        T[] array;

        public int Length { get { return this.length; } }
        public T this[int key]
        {
            get
            {
                key %= this.length;
                return this.array[key < 0 ? this.length + key : key];
            }
            set
            {
                key %= this.length;
                this.array[key < 0 ? this.length + key : key] = value;
            }
        }

        public CyclicArray(int length)
        {
            this.length = length;
            this.array = new T[length];
        }

        public CyclicArray(T[] array)
        {
            this.array = array;
            this.length = array.Length;
        }

        public void SetArray(T[] array)
        {
            this.array = array;
            this.length = array.Length;
        }
    }
}