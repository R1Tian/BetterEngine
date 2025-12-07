using System;
using System.Collections.Generic;
using Game.EventMgr;

namespace Game.ECWorld
{
    public class ECEntityFactory
    {
        private readonly Dictionary<Type, ECEntityPool> _poolMap = new();

        public T GetEntity<T>() where T : ECEntity, new()
        {
            Type entityType = typeof(T);
            _poolMap.TryGetValue(entityType, out var pool);

            ECEntityPool<T> poolT;
            if (pool != null)
            {
                poolT = (ECEntityPool<T>)pool;
            }
            else
            {
                poolT = new ECEntityPool<T>(this);
                _poolMap[entityType] = poolT;
            }

            return (T)poolT.GetEntity();
        }

        public void RecycleEntity(ECEntity entity)
        {
            if(entity == null) return;
            if(!_poolMap.TryGetValue(entity.GetType(), out var pool)) return;
            pool.RecycleEntity(entity);
        }
        
        private int _allocatedId = 0;

        public int GetInsID()
        {
            return ++_allocatedId;
        }
    }
}