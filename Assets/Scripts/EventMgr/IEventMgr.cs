using System;

namespace Game.EventMgr
{
    public interface IEventMgr
    {
        public void Init();

        public void Clear();
        
        public void Destroy();

        public void Update();

        #region Dispatch

        public void Dispatch(string eventName, EventDataBase eventDataBase);
        public void DispatchAsync(string eventName, EventDataBase eventDataBase);

        #endregion

        #region AddListener

        public void AddListener(string eventName, Action<EventDataBase> callback, object target = null);
        public void AddListenerOnce(string eventName, Action<EventDataBase> callback, object target);

        #endregion

        #region RemoveListener

        public void RemoveListener(string eventName, Action<EventDataBase> callback, object target);
        public void RemoveListenerByTarget(object target);
        public void RemoveListenerByEventName(string eventName, object target);

        #endregion
    }
}