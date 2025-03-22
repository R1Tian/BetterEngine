using System.Collections.Generic;

namespace Better.Collections
{
    public class BSetPool<T>
    {
        readonly BArrayPool<bool> _existsPool = new BArrayPool<bool>();
        readonly BArrayPool<T> _itemsPool = new BArrayPool<T>();
        readonly BArrayPool<BSet<T>.OperateItem> _operateItemsPool = new BArrayPool<BSet<T>.OperateItem>();
        readonly Stack<BSet<T>> _stacks = new Stack<BSet<T>>();

        static BSetPool<T> _default;
        public static BSetPool<T> Default
        {
            get
            {
                if (null == _default)
                {
                    _default = new BSetPool<T>();
                }

                return _default;
            }
        }
        
        public BSet<T> Get(int initNum = 0)
        {
            while (_stacks.Count > 0)
            {
                var set = _stacks.Pop();
                if (null == set) continue;

                set.Renew(initNum);

                return set;
            }

            return new BSet<T>(_existsPool, _itemsPool, _operateItemsPool, initNum);
        }

        public void Recycle(BSet<T> set)
        {
            if (null == set) return;
            if (set.IsRecycled) return;

            set.Recycle();

#if !BAMBOO_COLLECTIONS_RELEASE && (BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR)
            if (_stacks.Contains(set)) UnityEngine.Debug.LogError($"{set} is already in the BSetPool<{typeof(T)}>.");
#endif

            _stacks.Push(set);
        }
    }
}