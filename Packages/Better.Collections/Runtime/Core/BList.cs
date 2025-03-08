namespace Better.Collections
{
    public class BList<T>
    {
        const int MIN_CAPACITY = 8; // BList 最小容量
        
        private BArrayPool<T> _itemsPool;
        private BArrayPool<OperateItem> _operateItemsPool;
        private BArray<T> _items;
        
        public bool IsRecycled { get; private set; }
        private int _version;
        
        private int _count; // 元素数量
        
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