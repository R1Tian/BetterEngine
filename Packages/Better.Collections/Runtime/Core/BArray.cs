namespace Better.Collections
{
    public class BArray<T>
    {
        private readonly BinaryArrayPool<T> _itemsPool;

        public bool IsRecycled { get; private set; }
        private int _version;
        
        private int _capacity; // 容量

        private T[] _items;

        public int Capacity
        {
            get
            {
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get Capacity : {_capacity}");
#endif
                }
                
                return _capacity;
            }
        }
        
        public T this[int index]
        {
            get
            {
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get index : {index}");
#endif
                    return default;
                }

                if (index >= _capacity) throw new System.ArgumentOutOfRangeException(nameof(index), index, string.Empty);

                return _items[index];
            }
            set
            {
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. set value : {value}");
#endif
                    return;
                }

                if (index >= _capacity) throw new System.ArgumentOutOfRangeException(nameof(index), index, string.Empty);

                _items[index] = value;
            }
        }

        internal BArray(int length, BinaryArrayPool<T> itemsPool)
        {
            _itemsPool = itemsPool;
            
            IsRecycled = false;
            _version = 1;
            _capacity = BinaryHelper.GetAlignSize(length);
            _items = _itemsPool.Get(length);
        }

        internal void Renew(int length)
        {
            if (!IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has not been recycled. Renew(length) : {length}");
#endif
                return;
            }

            IsRecycled = false;
            _version++;
            _capacity = BinaryHelper.GetAlignSize(length);
            _items = _itemsPool.Get(length);
        }

        internal void Recycle()
        {
            if (IsRecycled) return;

            IsRecycled = true;
            _version++;

            _itemsPool.Recycle(_items);
            _items = null;
            
            _capacity = 0;
        }

        public void ResetCapacity(int length)
        {
            if (!IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has not been recycled. ResetLength(int length) : {length}");
#endif
                return;
            }
            
            var newCapacity = BinaryHelper.GetAlignSize(length);
            if (_capacity != newCapacity)
            {
                var newItems = _itemsPool.Get(newCapacity);
                System.Array.Copy(_items, 0, newItems, 0, System.Math.Min(_capacity, newCapacity));
                _itemsPool.Recycle(_items);
                _items = newItems;
                
                _capacity = newCapacity;
            }
        }

        public bool Contains(T item)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has not been recycled. Contains(T item) : {item}");
#endif
                return false;
            }
            
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has not been recycled. IndexOf(T item) : {item}");
#endif
            }

            return System.Array.IndexOf(_items, item, 0, _capacity);
        }

        public void Clear()
        {
            if (!IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has not been recycled. Clear()");
#endif
                return;
            }
            
            System.Array.Clear(_items, 0, _capacity);
        }
    }
}