using System.Collections.Generic;

namespace Better.Collections
{
    public class BListPool<T>
    {
        private readonly BArrayPool<T> _itemsPool = new BArrayPool<T>();
        readonly BArrayPool<BList<T>.OperateItem> _operateItemsPool = new BArrayPool<BList<T>.OperateItem>();
        private readonly Stack<BList<T>> _stacks = new Stack<BList<T>>();
        
        private static BListPool<T> _default;
        public static BListPool<T> Default => _default ??= new BListPool<T>();

        public BList<T> Get(int initNum = 0)
        {
            while (_stacks.Count > 0)
            {
                var list = _stacks.Pop();
                if (list == null) continue;
                
                list.Renew(initNum);
                
                return list;
            }
            
            return new BList<T>(_itemsPool, _operateItemsPool, initNum);
        }

        public void Recycle(BList<T> list)
        {
            if (list == null) return;
            if (list.IsRecycled) return;

            list.Recycle();

#if !BAMBOO_COLLECTIONS_RELEASE && (BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR)
            if (_stacks.Contains(list)) UnityEngine.Debug.LogError($"{list} is already in the BListPool<{typeof(T)}>.");
#endif

            _stacks.Push(list);
        }
    }
}