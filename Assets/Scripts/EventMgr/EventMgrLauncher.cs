using System;
using Better.Chronos;

namespace Game.EventMgr
{
    public class EventMgrLauncher<T> where T : class, IEventMgr, new()
    {
        public T EventMgr { get; private set; }
        public EventDataFactory EventDataFactory { get; private set; }

        public void Init()
        {
            EventMgr = new T();
            EventMgr.Init();
            EventDataFactory = new EventDataFactory();

            UpdateTimer.Default.LoopFrame(1, Update);
        }

        private void Update()
        {
            EventMgr.Update();
        }

        public void Clear()
        {
            EventMgr.Clear();
        }

        public void Destroy()
        {
            EventMgr = null;
            EventDataFactory = null;
        }
    }
}