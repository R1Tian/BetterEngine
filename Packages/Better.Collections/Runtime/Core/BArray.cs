namespace Better.Collections
{
    public class BArray<T>
    {
        private readonly BinaryArrayPool<T> _itemsPool;

        public bool IsRecycled { get; private set; }
        private int _version;

        private int _length; // 长度
        private int _capacity; // 容量

        private T[] _items;
        
        public int Length
        {
            get
            {
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get Length : {_length}");
#endif
                }
                
                return _length;
            }
        }

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
            _length = length;
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
            _length = length;
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
            
            _length = _capacity = 0;
        }
        
        public void ResetLength(int length)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. ResetLength(length) : {length}");
#endif
                return;
            }

            _length = length;

            int oldCapacity = _capacity;
            _capacity = BinaryHelper.GetAlignSize(length);

            if (_capacity != oldCapacity)
            {
                T[] newItems = _itemsPool.Get(length);
                System.Array.Copy(_items, 0, newItems, 0, System.Math.Min(_capacity, oldCapacity));
                _itemsPool.Recycle(_items);
                _items = newItems;
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

            return System.Array.IndexOf(_items, item, 0, _length);
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

        public void Remove(T item)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. Remove(item) : {item}");
#endif
                return;
            }
            
            int index = IndexOf(item);
            if (index < 0) return;

            RemoveAt(index);
        }
        
        public void RemoveAt(int index)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. RemoveAt(index) : {index}");
#endif
                return;
            }

            if (index < 0 || index >= _length)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), index, string.Empty);
            }
            
            _length--;
            
            int oldCapacity = _capacity;
            _capacity = BinaryHelper.GetAlignSize(_length);

            if (_capacity != oldCapacity)
            {
                T[] newItems = _itemsPool.Get(_length);
                if (index > 0) System.Array.Copy(_items, 0, newItems, 0, index);
                if (index < _length) System.Array.Copy(_items, index + 1, newItems, index, _length - index);
                _itemsPool.Recycle(_items);
                _items = newItems;
            }
            else
            {
                if (index < _length) System.Array.Copy(_items, index + 1, _items, index, _length - index);
                _items[_length] = default;
            }
        }
    }
}