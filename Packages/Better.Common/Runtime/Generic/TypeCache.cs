using System;

namespace Better
{
    public static class TypeCache<T>
    {
        public static readonly Type Type = typeof(T);
        public static readonly RuntimeTypeHandle TypeHandle = typeof(T).TypeHandle;
        public static readonly Lazy<Type> LazyType = new(() => typeof(T));

        static TypeCache()
        {
            
        }
    }
}