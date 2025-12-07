using System;
using System.Collections.Generic;
using Better;

namespace Game.EventMgr
{
    public class EventDataFactory
    {
        private readonly Dictionary<Type, RecyclePool> _poolMap = new();

        public EventData<T> GetEventData<T>()
        {
            Type eventDataType = typeof(EventData<T>);
            _poolMap.TryGetValue(eventDataType, out RecyclePool pool);

            RecyclePool<EventData<T>> poolT;
            if (pool != null)
            {
                poolT = (RecyclePool<EventData<T>>)pool;
            }
            else
            {
                poolT = new RecyclePool<EventData<T>>();
                _poolMap[eventDataType] = poolT;
            }

            return (EventData<T>)poolT.GetObject();
        }

        public EventData<T1, T2> GetEventData<T1, T2>()
        {
            Type eventDataType = typeof(EventData<T1, T2>);
            _poolMap.TryGetValue(eventDataType, out RecyclePool pool);

            RecyclePool<EventData<T1, T2>> poolT;
            if (pool != null)
            {
                poolT = (RecyclePool<EventData<T1, T2>>)pool;
            }
            else
            {
                poolT = new RecyclePool<EventData<T1, T2>>();
                _poolMap[eventDataType] = poolT;
            }

            return (EventData<T1, T2>)poolT.GetObject();
        }

        public EventData<T1, T2, T3> GetEventData<T1, T2, T3>()
        {
            Type eventDataType = typeof(EventData<T1, T2>);
            _poolMap.TryGetValue(eventDataType, out RecyclePool pool);

            RecyclePool<EventData<T1, T2>> poolT;
            if (pool != null)
            {
                poolT = (RecyclePool<EventData<T1, T2>>)pool;
            }
            else
            {
                poolT = new RecyclePool<EventData<T1, T2>>();
                _poolMap[eventDataType] = poolT;
            }

            return (EventData<T1, T2, T3>)poolT.GetObject();
        }

        public EventData<T1, T2, T3, T4> GetEventData<T1, T2, T3, T4>()
        {
            Type eventDataType = typeof(EventData<T1, T2>);
            _poolMap.TryGetValue(eventDataType, out RecyclePool pool);

            RecyclePool<EventData<T1, T2>> poolT;
            if (pool != null)
            {
                poolT = (RecyclePool<EventData<T1, T2>>)pool;
            }
            else
            {
                poolT = new RecyclePool<EventData<T1, T2>>();
                _poolMap[eventDataType] = poolT;
            }

            return (EventData<T1, T2, T3, T4>)poolT.GetObject();
        }

        public EventData<T1, T2, T3, T4, T5> GetEventData<T1, T2, T3, T4, T5>()
        {
            Type eventDataType = typeof(EventData<T1, T2>);
            _poolMap.TryGetValue(eventDataType, out RecyclePool pool);

            RecyclePool<EventData<T1, T2>> poolT;
            if (pool != null)
            {
                poolT = (RecyclePool<EventData<T1, T2>>)pool;
            }
            else
            {
                poolT = new RecyclePool<EventData<T1, T2>>();
                _poolMap[eventDataType] = poolT;
            }

            return (EventData<T1, T2, T3, T4, T5>)poolT.GetObject();
        }

        public void Recycle(EventDataBase eventData)
        {
            Type type = eventData.GetType();

            if (_poolMap.TryGetValue(type, out var pool))
            {
                pool.RecycleObject(eventData);
            }
        }
    }
}