namespace Better.Collections
{
    public class BList<T>
    {
        private BArrayPool<T> _itemsPool;
        private BArray<T> _items;

        internal BList(BArrayPool<T> itemsPool)
        {
            
        }
    }
}