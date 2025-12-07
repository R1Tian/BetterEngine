using System;
using Better.Chronos;
using Game.EventMgr;
using UnityEngine;

namespace Runtime
{
    public class GameStart : MonoBehaviour
    {
        private EventMgrLauncher<DefaultEventMgr> _eventMgrLauncher;
        
        private void Start()
        {
            _eventMgrLauncher = new();
            _eventMgrLauncher.Init();

            var data = _eventMgrLauncher.EventDataFactory.GetEventData<int, string>();
            data.SetData(1, "23");
            _eventMgrLauncher.EventMgr.AddListener("Test", OnEvent);
            
            _eventMgrLauncher.EventMgr.DispatchAsync("Test", data);
            _eventMgrLauncher.EventDataFactory.Recycle(data);
        }

        private void OnEvent(EventDataBase @event)
        {
            var arg = @event as EventData<int,string>;
            
            Debug.Log(arg.Data1);
            Debug.Log(arg.Data2);
        }
    }
}