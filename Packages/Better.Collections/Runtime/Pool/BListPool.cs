using System.Collections.Generic;

namespace Better.Collections
{
    public class BListPool<T>
    {
        private readonly BArrayPool<T> _itemsPool = new BArrayPool<T>();
        private readonly Stack<BArray<T>> _stacks = new Stack<BArray<T>>();
        
        private static BListPool<T> _default;
        public static BListPool<T> Default => _default ??= new BListPool<T>();

        public BList<T> Get(int length)
        {
            return null;
        }

        public void Recycle(BList<T> list)
        {
            
        }
    }
}