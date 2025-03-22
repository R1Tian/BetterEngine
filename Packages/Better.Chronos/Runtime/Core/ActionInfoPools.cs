using System;
using System.Collections.Generic;

namespace Better.Chronos
{
    internal sealed class ActionInfoPools
    {
        private readonly Dictionary<RuntimeTypeHandle, RecyclePool> _poolMap = new();

        public T Get<T>() where T: TypeRecycle, new()
        {
            _poolMap.TryGetValue(TypeCache<T>.TypeHandle, out var pool);

            RecyclePool<T> poolT;
            if (pool == null)
            {
                poolT = new RecyclePool<T>();
                _poolMap[TypeCache<T>.TypeHandle] = poolT;
            }
            else
            {
                poolT = (RecyclePool<T>)pool;
            }
            
            return poolT.Get();
        }

        public void Recycle(TypeRecycle typeRecycle)
        {
            if (typeRecycle == null) return;

            if (!_poolMap.TryGetValue(typeRecycle.TypeHandle, out var pool)) return;

            pool?.RecycleObject(typeRecycle);
        }
    }
}