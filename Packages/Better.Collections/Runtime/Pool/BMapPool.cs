using System.Collections.Generic;

namespace Better.Collections
{
    public class BMapPool<TKey, TValue>
    {
        readonly BArrayPool<bool> _existsPool = new BArrayPool<bool>();
        readonly BArrayPool<TKey> _keysPool = new BArrayPool<TKey>();
        readonly BArrayPool<TValue> _valuesPool = new BArrayPool<TValue>();
        readonly BArrayPool<BMap<TKey, TValue>.OperateItem> _operateItemsPool = new BArrayPool<BMap<TKey, TValue>.OperateItem>();
        readonly Stack<BMap<TKey, TValue>> _stack = new Stack<BMap<TKey, TValue>>();
        
        static BMapPool<TKey, TValue> _default;
        public static BMapPool<TKey, TValue> Default => _default ??= new BMapPool<TKey, TValue>();

        public BMap<TKey, TValue> Get(int initNum = 0)
        {
            while (_stack.Count > 0)
            {
                var bMap = _stack.Pop();
                if(bMap == null) continue;
                
                bMap.Renew(initNum);
                return bMap;
            }
            
            return new BMap<TKey, TValue>(_existsPool, _keysPool, _valuesPool, _operateItemsPool, initNum);
        }

        public void Recycle(BMap<TKey, TValue> bMap)
        {
            if (bMap == null) return;
            if (bMap.IsRecycled) return;

            bMap.Recycle();

#if !BAMBOO_COLLECTIONS_RELEASE && (BAMBOO_COLLECTIONS_DEBUG || UNITY_EDITOR)
            if (_stack.Contains(bMap)) UnityEngine.Debug.LogError($"{bMap} is already in the BMapPool<{typeof(TKey)}, {typeof(TValue)}>.");
#endif

            _stack.Push(bMap);
        }
    }
}