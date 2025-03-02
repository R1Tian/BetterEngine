using System.Collections.Generic;

namespace Better
{
    public sealed class BinaryArrayPool<T>
    {
        readonly int _minBit; // 数组最小长度 2^_minBit
        readonly int _maxBit; // 数组最大长度 2^_maxBit
        readonly Stack<T[]>[] _stacks; // [0] 放容量为 _minBit，(2^n, 2^n+1]
        
        public BinaryArrayPool(int minBit, int maxBit)
        {
            _minBit = minBit;
            _maxBit = maxBit;

            var stackCount = _maxBit - _minBit + 1;
            _stacks = new Stack<T[]>[stackCount];
            for (int i = 0; i < stackCount; i++)
            {
                _stacks[i] = new Stack<T[]>();
            }
        }

        public T[] Get(int length)
        {
            int bit = -1;
            int capacity = 0;
            for (int i = _minBit; i <= _maxBit; i++)
            {
                capacity = BinaryHelper.GetPower2(i);
                if (length <= capacity)
                {
                    bit = i;
                    break;
                }
            }

            if (bit == -1) return new T[length]; // 超出池子存储范围

            var stack = _stacks[bit - _minBit];
            while (stack.Count > 0)
            {
                var array = stack.Pop();
                if (array == null) continue;

                return array;
            }

            return new T[capacity];
        }

        public void Recycle(T[] items)
        {
            if(items == null) return;
            
            System.Array.Clear(items, 0, items.Length);

            int bit = -1;
            for (int i = _minBit; i <= _maxBit; i++)
            {
                if (items.Length == BinaryHelper.GetPower2(i))
                {
                    bit = i;
                    break;
                }
            }
            
            if (bit == -1) return; // 超出池子存储范围
            
            var stack = _stacks[bit - _minBit];

#if !BAMBOO_COLLECTIONS_RELEASE && (BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR)
            if (stack.Contains(items))
            {
                UnityEngine.Debug.LogError($"{items} is already in the BinaryArrayPool<{typeof(T)}>.");
            }
#endif

            stack.Push(items);
        }
    }
}