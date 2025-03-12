using System.Collections;
using System.Collections.Generic;

namespace Better.Collections
{
    public class BMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        const int MIN_LENGTH = 8;
        
        readonly BArrayPool<bool> _existsPool;
        readonly BArrayPool<TKey> _keysPool;
        readonly BArrayPool<TValue> _valuesPool;
        readonly BArrayPool<OperateItem> _operateItemsPool;
        
        BArray<bool> _exists;
        BArray<TKey> _keys;
        BArray<TValue> _values;
        BArray<OperateItem> _operateItems;
        
        public bool IsRecycled { get; private set; }
        public int Version { get; private set; }
        
        public TValue this[TKey key]
        {
            get
            {
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get key : {key}");
#endif
                    return default;
                }

                int index = FindEntry(key);
                if (index < 0) throw new System.Exception($"{key} didn't exist.");

                return _values[index];
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

                if (_iterateCount > 0)
                {
                    InnerSetToQueue(key, value);
                }
                else
                {
                    InnerSet(key, value);
                }
            }
        }

        int _iterateCount;
        
        int _count;
        public int Count
        {
            get
            {
                if (IsRecycled)
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
                if (IsRecycled)
                {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError($"This container has been recycled. get Capacity : {_keys.Length}");
#endif
                    return 0;
                }

                return _keys.Length;
            }
        }
        
        int _lastIndex; // _exists 中查找可用的 index 时，从 _lastIndex + 1 开始往后查找，后面没有可用的 index 时，从 0 开始查找

        internal BMap(
            BArrayPool<bool> existsPool,
            BArrayPool<TKey> keysPool,
            BArrayPool<TValue> valuesPool,
            BArrayPool<OperateItem> operateItemsPool,
            int initNum = 0)
        {
            Version = 1;

            _existsPool = existsPool;
            _keysPool = keysPool;
            _valuesPool = valuesPool;
            _operateItemsPool = operateItemsPool;

            int length = System.Math.Max(MIN_LENGTH, initNum + (initNum >> 1));

            _exists = _existsPool.Get(length);
            _keys = _keysPool.Get(length);
            _values = _valuesPool.Get(length);

            _count = 0;
            _lastIndex = -1;
        }

        internal void Renew(int initNum = 0)
        {
            if (!IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError("This container has not been recycled. Renew()");
#endif
                return;
            }
            
            IsRecycled = false;
            Version++;
            
            int length = System.Math.Max(MIN_LENGTH, initNum + (initNum >> 1));
            
            _exists = _existsPool.Get(length);
            _keys = _keysPool.Get(length);
            _values = _valuesPool.Get(length);

            _count = 0;
            _lastIndex = -1;
        }

        internal void Recycle()
        {
            if (_iterateCount > 0) throw new System.Exception("Do not operate during iteration.");
            
            IsRecycled = true;
            Version++;

            _existsPool.Recycle(_exists);
            _exists = null;
            
            _keysPool.Recycle(_keys);
            _keys = null;

            _valuesPool.Recycle(_values);
            _values = null;
            
            _count = 0;
            _lastIndex = -1;
        }
        
        public bool ContainsKey(TKey key)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. ContainsKey(key) : {key}");
#endif
                return false;
            }

            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. ContainsValue(value) : {value}");
#endif
                return false;
            }

            var comparer = EqualityComparer<TValue>.Default;

            for (int i = 0; i < _values.Length; ++i)
            {
                if (!_exists[i]) continue;

                if (comparer.Equals(_values[i], value)) return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. TryGetValue(key) : {key}");
#endif
                value = default;
                return false;
            }

            int index = FindEntry(key);
            if (index < 0)
            {
                value = default;
                return false;
            }

            value = _values[index];
            return true;
        }

        int FindEntry(TKey key)
        {
            var comparer = EqualityComparer<TKey>.Default;
            for (int i = 0; i < _keys.Length; i++)
            {
                if (!_exists[i]) continue;
                if (comparer.Equals(_keys[i], key)) return i;
            }

            return -1;
        }

        #region Set
        void InnerSet(TKey key, TValue value)
        {
            var comparer = EqualityComparer<TKey>.Default;
            for (int i = 0; i < _keys.Length; i++)
            {
                if (!_exists[i]) continue;
                if (comparer.Equals(_keys[i], key))
                {
                    _values[i] = value;

                    return;
                }
            }
        }

        void InnerSetToQueue(TKey key, TValue value)
        {
            if (null == _operateItems)
            {
                _operateItems = _operateItemsPool.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Set,
                    Key = key,
                    Value = value
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Set,
                    Key = key,
                    Value = value
                };
            }
        }

        #endregion
        
        #region Add
        public void Add(TKey key, TValue value)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. Add(key, value) : {key} {value}");
#endif
                return;
            }

            if (_iterateCount > 0)
            {
                InnerAddToQueue(key, value);
            }
            else
            {
                InnerAdd(key, value);
            }
        }
        
        void InnerAdd(TKey key, TValue value)
        {
            var comparer = EqualityComparer<TKey>.Default;

            for (int i = 0; i < _keys.Length; i++)
            {
                if (!_exists[i]) continue;

                if (comparer.Equals(_keys[i], key))
                {
                    throw new System.Exception($"The key already exists. key : {key}");
                }
            }
            
            _count++;
            
            var length = _keys.Length;
            if (_count + (_count >> 1) > length)
            {
                length *= 2;
                _keys.ResetLength(length);
                _values.ResetLength(length);
                _exists.ResetLength(length);
            }

            var index = _lastIndex + 1;
            for (int i = index; i < _keys.Length; i++)
            {
                if (_exists[i]) continue;

                _keys[i] = key;
                _values[i] = value;
                _exists[i] = true;
                
                _lastIndex = i;

                return;
            }

            for (int i = 0; i < index; i++)
            {
                if (_exists[i]) continue;

                _keys[i] = key;
                _values[i] = value;
                _exists[i] = true;
                
                _lastIndex = i;

                return;
            }
            
            throw new System.Exception($"Impossible Exception." +
                                       $" _count : {_count}" +
                                       $" _keys.Length : {_keys.Length}" +
                                       $" _keys.Capacity : {_keys.Capacity}" +
                                       $" _lastIndex : {_lastIndex}");
        }

        void InnerAddToQueue(TKey key, TValue value)
        {
            if (null == _operateItems)
            {
                _operateItems = _operateItemsPool.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Add,
                    Key = key,
                    Value = value
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Add,
                    Key = key,
                    Value = value
                };
            }
        }

        #endregion
        
        #region Remove
        public bool Remove(TKey key)
        {
            if (IsRecycled)
            {
#if BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR
                UnityEngine.Debug.LogError($"This container has been recycled. Remove(key) : {key}");
#endif
                return false;
            }

            if (_iterateCount > 0)
            {
                InnerRemoveToQueue(key);

                return false;
            }
            else
            {
                return InnerRemove(key);
            }
        }
        
        bool InnerRemove(TKey key)
        {
            var comparer = EqualityComparer<TKey>.Default;

            for (int i = 0; i < _keys.Length; i++)
            {
                if(!_exists[i]) continue;

                if (comparer.Equals(_keys[i], key))
                {
                    _keys[i] = default;
                    _values[i] = default;
                    _exists[i] = false;

                    _count--;

                    return true;
                }
            }
            
            return false;
        }

        void InnerRemoveToQueue(TKey key)
        {
            if (null == _operateItems)
            {
                _operateItems = _operateItemsPool.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Remove,
                    Key = key,
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Remove,
                    Key = key,
                };
            }
        }
        
        #endregion
        
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
            
            for (int i = 0; i < _operateItems.Length; i++)
            {
                switch (_operateItems[i].OperateType)
                {
                    case OperateType.Set:
                        InnerSet(_operateItems[i].Key, _operateItems[i].Value);
                        break;
                    case OperateType.Add:
                        InnerAdd(_operateItems[i].Key, _operateItems[i].Value);
                        break;
                    case OperateType.Remove:
                        InnerRemove(_operateItems[i].Key);
                        break;
                }
            }

            _operateItemsPool.Recycle(_operateItems);
            _operateItems = null;
        }

        #region Enumerator
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

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
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
        
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            readonly BMap<TKey, TValue> _map;
            KeyValuePair<TKey, TValue> _current;
            int _index;

            public KeyValuePair<TKey, TValue> Current { get { return _current; } }
            object IEnumerator.Current { get { return _current; } }

            public Enumerator(BMap<TKey, TValue> map)
            {
                _map = map;
                _current = default;
                _index = 0;

                if (null == _map) return;

                ++_map._iterateCount;
            }

            public void Dispose()
            {
                if (null == _map) return;

                --_map._iterateCount;
                if (_map._iterateCount == 0)
                {
                    _map.ExecuteOperateQueue();
                }
            }

            public bool MoveNext()
            {
                if (null == _map) return false;

                if (_map.Count == 0) return false;

                int length = _map._keys.Length;
                for (int i = _index; i < length; ++i)
                {
                    if (!_map._exists[i]) continue;

                    _current = new KeyValuePair<TKey, TValue>(_map._keys[i], _map._values[i]);
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
        
        #endregion
        
        internal enum OperateType
        {
            Add = 0, // 加入新的 kv
            Set = 1, // 设置已有的 kv
            Remove = 2,
        }

        internal struct OperateItem
        {
            public OperateType OperateType;
            public TKey Key;
            public TValue Value;
        }
    }
}