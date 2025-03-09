using System.Collections;
using System.Collections.Generic;

namespace Better.Collections
{
    public class BList<T> : IEnumerable<T>
    {
        const int MIN_CAPACITY = 8; // BList 最小容量
        
        private BArrayPool<T> _itemsPool;
        private BArrayPool<OperateItem> _operateItemsPool;
        
        private BArray<T> _items;
        
        private BArray<OperateItem> _operateItems;
        private int _iterateCount; // 遍历者数量
        
        public bool IsRecycled { get; private set; }
        private int _version;
        
        private int _count; // 元素数量
        public int Count
        {
            get
            {
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get Count : {_count}");
#endif
                }
                
                return _count;
            }
        }
        
        public int Capacity
        {
            get
            {
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get Capacity : {_items.Capacity}");
#endif
                }
                
                return _items.Capacity;
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

                if (index >= _count) throw new System.ArgumentOutOfRangeException(nameof(index), index, string.Empty);

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

                if (index >= _count) throw new System.ArgumentOutOfRangeException(nameof(index), index, string.Empty);

                _items[index] = value;
            }
        }
        
        internal BList(BArrayPool<T> itemsPool, BArrayPool<OperateItem> operateItemsPool, int initNum = 0)
        {
            _itemsPool = itemsPool;
            _operateItemsPool = operateItemsPool;
            
            IsRecycled = false;
            _version = 1;

            _items = _itemsPool.Get(initNum == 0
                ? MIN_CAPACITY
                : System.Math.Max(MIN_CAPACITY, initNum + (initNum >> 1))); // 初始设置为 1.5 倍

            _count = 0;
        }

        internal void Renew(int initNum = 0)
        {
            if (!IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has not been recycled. Renew(initNum) : {initNum}");
#endif
                return;
            }

            IsRecycled = false;
            _version++;
            _items = _itemsPool.Get(System.Math.Max(MIN_CAPACITY, initNum));
            _count = 0;
        }

        internal void Recycle()
        {
            if (IsRecycled) return;

            IsRecycled = true;
            _version++;

            _itemsPool.Recycle(_items);
            _items = null;

            _count = 0;
        }

        public bool Contains(T item)
        {
            if (!IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has not been recycled. Contains(item) : {item}");
#endif
                return false;
            }
            
            return _items.Contains(item);
        }

        public void Add(T item)
        {
            if (IsRecycled)
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

        private void InnerAdd(T item)
        {
            _count++;
            
            var length = _items.Length;
            if (_count > length)
            {
                length *= 2;
                _items.ResetLength(length);
            }

            _items[_count - 1] = item;
        }

        private void InnerAddToQueue(T item)
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

        public bool Remove(T item)
        {
            if (IsRecycled)
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

        private bool InnerRemove(T item)
        {
            int index = _items.IndexOf(item);
            if (index < 0 || index >= _count) return false;
            
            _count--;
            _items.RemoveAt(index);
            return true;
        }

        private void InnerRemoveToQueue(T item)
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

            if (IsRecycled)
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
            if (IsRecycled)
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
            if (IsRecycled)
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
            if (IsRecycled)
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
            readonly BList<T> _list;
            T _current;
            int _index;

            public T Current { get { return _current; } }
            object IEnumerator.Current { get { return _current; } }

            public Enumerator(BList<T> list)
            {
                _list = list;
                _current = default;
                _index = 0;

                if (null == _list) return;

                ++_list._iterateCount;
            }

            public void Dispose()
            {
                if (null == _list) return;

                --_list._iterateCount;
                if (_list._iterateCount == 0)
                {
                    _list.ExecuteOperateQueue();
                }
            }

            public bool MoveNext()
            {
                if (null == _list) return false;

                if (_list.Count == 0) return false;

                if (_index >= _list._count) return false;

                _current = _list._items[_index++];

                return true;
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