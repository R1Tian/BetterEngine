using System;
using System.Collections.Generic;
using Better;

namespace Game.EventMgr
{
    public class DefaultEventMgr : IEventMgr
    {
        private EventRecorder _recorder;
        
        private List<CachedDispatchEvent> _cachedEventList;

        private bool _isRunning;

        private const int MaxCount = 100; // 一帧处理的事件的最大数量

        #region 生命周期

        public void Init()
        {
            _recorder = new();
            _cachedEventList = new();
            _runningEventList = new();
            _isRunning = true;
        }
        
        public void Clear()
        {
            
        }

        public void Destroy()
        {
            _recorder = null;
            ClearCachedEventList();
            _cachedEventList = null;
            _isRunning = false;
        }
        
        private class CachedDispatchEvent : IRecycle
        {
            public bool IsRecycled { get; set; }
            public void Recycle()
            {
                EventName = null;
                EventData = null;
            }
            
            public string EventName { get; private set; }
            public EventDataBase EventData { get; private set; }

            public void SetData(string eventName, EventDataBase eventData)
            {
                EventName = eventName;
                EventData = eventData;
            }
        }
        private List<CachedDispatchEvent> _runningEventList;
        public void Update()
        {
            if(!_isRunning) return;
            
            _recorder.CheckInvalidEventRecord();
            
            if(_cachedEventList.Count == 0) return;
            
            var count = Math.Min(_cachedEventList.Count, MaxCount);
            for (int i = 0; i < count; i++)
            {
                _runningEventList.Add(_cachedEventList[i]);
            }
            _cachedEventList.RemoveRange(0, count);
            _isRunning = _cachedEventList.Count > 0;

            foreach (var cachedEventData in _runningEventList)
            {
                _recorder.Dispatch(cachedEventData.EventName, cachedEventData.EventData);
                RecyclePool<CachedDispatchEvent>.Default.Recycle(cachedEventData);
            }
            _runningEventList.Clear();
        }

        #endregion

        #region Dispatch

        public void Dispatch(string eventName, EventDataBase eventDataBase)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }
            
            _recorder.Dispatch(eventName, eventDataBase);
        }
        
        public void DispatchAsync(string eventName, EventDataBase eventDataBase)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            var cachedEventData = RecyclePool<CachedDispatchEvent>.Default.Get();
            cachedEventData.SetData(eventName, eventDataBase);
            _cachedEventList.Add(cachedEventData);
            _isRunning = true;
        }

        #endregion

        #region AddListener

        public void AddListener(string eventName, Action<EventDataBase> callback, object target = null)
        {
            if (string.IsNullOrEmpty(eventName) || callback == null)
            {
                return;
            }
            
            _recorder.Add(eventName, callback, target);
        }

        public void AddListenerOnce(string eventName, Action<EventDataBase> callback, object target)
        {
            if (string.IsNullOrEmpty(eventName) || callback == null)
            {
                return;
            }
            
            _recorder.Add(eventName, callback, target, 1);
        }

        #endregion

        #region RemoveListener

        public void RemoveListener(string eventName, Action<EventDataBase> callback, object target)
        {
            _recorder.Remove(eventName, callback, target);
        }

        public void RemoveListenerByTarget(object target)
        {
            if (target == null) return;

            _recorder.RemoveByTarget(target);
        }

        public void RemoveListenerByEventName(string eventName, object target)
        {
            if (target == null) return;
            
            _recorder.RemoveByName(eventName, target);
        }

        #endregion

        private void ClearCachedEventList()
        {
            foreach (var cachedEventData in _cachedEventList)
            {
                RecyclePool<CachedDispatchEvent>.Default.Recycle(cachedEventData);
            }
            
            _cachedEventList.Clear();
        }
    }

    
}