using System.Collections;
using System.Collections.Generic;

namespace Better.Collections
{
    public class BSet<T> : IEnumerable<T>
    {
        const int MIN_LENGTH = 8;

        readonly BArrayPool<bool> _existsPool;
        readonly BArrayPool<T> _itemsPool;
        readonly BArrayPool<OperateItem> _operateItemsPool;

        BArray<bool> _exists;
        BArray<T> _items;

        BArray<OperateItem> _operateItems;
        int _iterateCount;

        bool _isRecycled;
        int _version;

        int _count;
        int _lastIndex;

        public bool IsRecycled { get { return _isRecycled; } }
        public int Version { get { return _version; } }

        public int Count
        {
            get
            {
                if (_isRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get Count : {_count}");
#endif
                    return 0;
                }

                return _count;
            }
        }

        public int Capacity
        {
            get
            {
                if (_isRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get Capacity : {_items.Length}");
#endif
                    return 0;
                }

                return _items.Length;
            }
        }
        
        internal BSet(
                      BArrayPool<bool> existsPool,
                      BArrayPool<T> itemsPool,
                      BArrayPool<OperateItem> operateItemsPool,
                      int initNum)
        {
            _version = 1;

            _existsPool = existsPool;
            _itemsPool = itemsPool;
            _operateItemsPool = operateItemsPool;

            int length = System.Math.Max(MIN_LENGTH, initNum + (initNum >> 1));

            _exists = _existsPool.Get(length);
            _items = _itemsPool.Get(length);

            _count = 0;
            _lastIndex = -1;
        }

        internal void Renew()
        {
            if (!_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has not been recycled. Renew()");
#endif
                return;
            }

            _isRecycled = false;
            ++_version;

            _exists = _existsPool.Get(MIN_LENGTH);
            _items = _itemsPool.Get(MIN_LENGTH);

            _count = 0;
            _lastIndex = -1;
        }

        internal void Renew(int initNum)
        {
            if (!_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has not been recycled. Renew(initNum) : {initNum}");
#endif
                return;
            }

            _isRecycled = false;
            ++_version;

            int length = System.Math.Max(MIN_LENGTH, initNum + (initNum >> 1));

            _exists = _existsPool.Get(length);
            _items = _itemsPool.Get(length);

            _count = 0;
            _lastIndex = -1;
        }

        internal void Recycle()
        {
            if (_iterateCount > 0) throw new System.Exception("Do not operate during iteration.");

            _isRecycled = true;
            ++_version;

            _existsPool.Recycle(_exists);
            _exists = null;

            _itemsPool.Recycle(_items);
            _items = null;
        }

        public bool Contains(T item)
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. Contains(item) : {item}");
#endif
                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < _items.Length; ++i)
            {
                if (!_exists[i]) continue;

                if (comparer.Equals(_items[i], item)) return true;
            }

            return false;
        }

        public void Add(T item)
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. Add(item) : {item}");
#endif
                return;
            }

            if (_iterateCount > 0)
            {
                InnerAddToQueue(item);
            }
            else
            {
                InnerAdd(item);
            }
        }

        public bool Remove(T item)
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. Remove(item) : {item}");
#endif
                return false;
            }

            if (_iterateCount > 0)
            {
                InnerRemoveToQueue(item);

                return false;
            }
            else
            {
                return InnerRemove(item);
            }
        }

        public void RemoveAt(int index)
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. RemoveAt(index) : {index}");
#endif
                return;
            }

            if (_iterateCount > 0) throw new System.Exception("Do not operate during iteration.");

            _items[index] = default;
            _exists[index] = false;

            --_count;
        }

        public void Clear()
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has been recycled. Clear()");
#endif
                return;
            }

            if (_iterateCount > 0) throw new System.Exception("Do not operate during iteration.");

            _items.Clear();
            _exists.Clear();

            _count = 0;
            _lastIndex = -1;
        }

        public void Compress()
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has been recycled. Compress()");
#endif
                return;
            }

            if (_iterateCount > 0) throw new System.Exception("Do not operate during iteration.");

            if (_count == 0)
            {
                _lastIndex = -1;
            }
            else
            {
                int index = 0;
                int len = _items.Length;

                for (int i = 0; i < len; ++i)
                {
                    if (!_exists[i]) continue;

                    _exists[i] = false;
                    _exists[index] = true;

                    if (i != index)
                    {
                        _items[index] = _items[i];
                        _items[i] = default;
                    }

                    ++index;
                }

                _lastIndex = index;
            }

            int length = System.Math.Max(MIN_LENGTH, _count + (_count >> 1));
            _items.ResetLength(length);
            _exists.ResetLength(length);
        }

        void InnerAdd(T item)
        {
            ++_count;

            int length = _items.Length;
            if (_count + (_count >> 1) > length)
            {
                length *= 2;
                _items.ResetLength(length);
                _exists.ResetLength(length);
            }

            int index = _lastIndex + 1;

            for (int i = index; i < length; ++i)
            {
                if (_exists[i]) continue;

                _exists[i] = true;
                _items[i] = item;

                _lastIndex = i;

                return;
            }

            for (int i = 0; i < index; ++i)
            {
                if (_exists[i]) continue;

                _exists[i] = true;
                _items[i] = item;

                _lastIndex = i;

                return;
            }

            throw new System.Exception($"Impossible Exception." +
                $" _count : {_count}" +
                $" _items.Length : {_items.Length}" +
                $" _items.Capacity : {_items.Capacity}" +
                $" _lastIndex : {_lastIndex}");
        }

        bool InnerRemove(T item)
        {
            int index = _items.IndexOf(item);
            if (index < 0) return false;

            _items[index] = default;
            _exists[index] = false;

            --_count;

            return true;
        }

        void InnerAddToQueue(T item)
        {
            if (null == _operateItems)
            {
                _operateItems = _operateItemsPool.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Add,
                    Item = item
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Add,
                    Item = item
                };
            }
        }

        void InnerRemoveToQueue(T item)
        {
            if (null == _operateItems)
            {
                _operateItems = _operateItemsPool.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Remove,
                    Item = item
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Remove,
                    Item = item
                };
            }
        }

        void ExecuteOperateQueue()
        {
            if (null == _operateItems) return;

            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has been recycled. ExecuteOperateQueue()");
#endif
                _operateItemsPool.Recycle(_operateItems);
                _operateItems = null;
                return;
            }

            int len = _operateItems.Length;
            for (int i = 0; i < len; ++i)
            {
                switch (_operateItems[i].OperateType)
                {
                    case OperateType.Add:
                        InnerAdd(_operateItems[i].Item);
                        break;
                    case OperateType.Remove:
                        InnerRemove(_operateItems[i].Item);
                        break;
                }
            }

            _operateItemsPool.Recycle(_operateItems);
            _operateItems = null;
        }

        public Enumerator GetEnumerator()
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has been recycled. GetEnumerator()");
#endif
                return new Enumerator(null);
            }

            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has been recycled. GetEnumerator()");
#endif
                return new Enumerator(null);
            }

            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_isRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has been recycled. GetEnumerator()");
#endif
                return new Enumerator(null);
            }

            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>
        {
            readonly BSet<T> _set;
            T _current;
            int _index;

            public T Current { get { return _current; } }
            object IEnumerator.Current { get { return _current; } }

            public Enumerator(BSet<T> set)
            {
                _set = set;
                _current = default;
                _index = 0;

                if (null == _set) return;

                ++_set._iterateCount;
            }

            public void Dispose()
            {
                if (null == _set) return;

                --_set._iterateCount;
                if (_set._iterateCount == 0)
                {
                    _set.ExecuteOperateQueue();
                }
            }

            public bool MoveNext()
            {
                if (null == _set) return false;

                if (_set.Count == 0) return false;

                int length = _set._items.Length;
                for (int i = _index; i < length; ++i)
                {
                    if (!_set._exists[i]) continue;

                    _current = _set._items[i];
                    _index = i + 1;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _index = 0;
                _current = default;
            }
        }

        internal enum OperateType
        {
            Add = 0,
            Remove = 1
        }

        internal struct OperateItem
        {
            public OperateType OperateType;
            public T Item;
        }
    }
}