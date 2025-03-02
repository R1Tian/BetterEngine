using System.Collections.Generic;

namespace Better.Collections
{
    public class BArrayPool<T>
    {
        private const int _minBit = 0; // 数组最小长度 2^_minBit
        private const int _maxBit = 10; // 数组最大长度 2^_maxBit
        
        private readonly BinaryArrayPool<T> _itemsPool = new BinaryArrayPool<T>(_minBit, _maxBit);
        private readonly Stack<BArray<T>> _stacks = new Stack<BArray<T>>();
        
        private static BArrayPool<T> _default;
        public static BArrayPool<T> Default => _default ??= new BArrayPool<T>();

        public BArray<T> Get(int length = 0)
        {
            while (_stacks.Count > 0)
            {
                var array = _stacks.Pop();
                if (array == null) continue;
                
                array.Renew(length);
                return array;
            }
            
            return new BArray<T>(length, _itemsPool);
        }

        public void Recycle(BArray<T> array)
        {
            if (array == null) return;
            if (array.IsRecycled) return;

            array.Recycle();
            
#if !BAMBOO_COLLECTIONS_RELEASE && (BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR)
            if (_stacks.Contains(array)) UnityEngine.Debug.LogError($"{array} is already in the BArrayPool<{typeof(T)}>.");
#endif
            
            _stacks.Push(array);
        }
    }
}