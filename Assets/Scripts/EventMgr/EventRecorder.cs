using System;
using System.Collections.Generic;
using Better;

namespace Game.EventMgr
{
    /// <summary>
    /// 事件记录者
    /// </summary>
    public class EventRecorder
    {
        private class EventRecord : IRecycle
        {
            public bool IsRecycled { get; set; }
            public void Recycle()
            {
                Name = null;
                Count = 0;
                Callback = null;
                Target = null;
                IsValid = false;
            }
        
            public string Name { get; private set; }
            
            public Action<EventDataBase> Callback { get; private set; }
            public object Target { get; private set; }
            public int Count { get; private set; }
            public bool IsValid { get; private set; }

            public void SetData(string name, Action<EventDataBase> callback, object target, int count)
            {
                Name = name;
                Callback = callback;
                Target = target;
                Count = count;
                IsValid = true;
            }

            public void SetInValid()
            {
                IsValid = false;
            }

            public void CheckCount()
            {
                if(Count == -1) return;

                Count--;
                if (Count <= 0)
                {
                    SetInValid();
                }
            }

            public bool IsEqual(string name, Action<EventDataBase> callback, object target)
            {
                return IsValid && Name == name && Callback == callback && Target == target;
            }
        }
        
        private Dictionary<string, List<EventRecord>> _eventMap;

        public EventRecorder()
        {
            _eventMap = new Dictionary<string, List<EventRecord>>();
        }

        public void Dispatch(string eventName, EventDataBase eventData)
        {
            var list = _eventMap[eventName];
            if (list == null || list.Count == 0) return;

            foreach (var eventRecord in list)
            {
                if (eventRecord != null && eventRecord.IsValid)
                {
                    eventRecord.CheckCount();
                    eventRecord.Callback(eventData);
                }
            }
        }
        
        public bool Add(string eventName, Action<EventDataBase> callback, object target = null, int count = -1)
        {
            if (string.IsNullOrEmpty(eventName) || callback == null)
            {
                return false;
            }
            
            var recordList = GetRecordList(eventName);
            if (recordList == null)
            {
                recordList = GetListFromPool();
                AddEventList(eventName, recordList);
            }
            else
            {
                foreach (var record in recordList)
                {
                    if (record != null && record.IsEqual(eventName, callback, target))
                    {
                        return false;
                    }
                }
            }

            var newRecord = RecyclePool<EventRecord>.Default.Get();
            newRecord.SetData(eventName, callback, target, count);
            recordList.Add(newRecord);
            return true;
        }

        public bool Remove(string eventName, Action<EventDataBase> callback, object target = null)
        {
            var list = GetRecordList(eventName);
            if (list == null || list.Count == 0) return false;

            foreach (var record in list)
            {
                if (record != null && record.IsEqual(eventName, callback, target))
                {
                    record.SetInValid();
                }
            }

            return true;
        }

        public bool RemoveByName(string eventName, object target)
        {
            var list = GetRecordList(eventName);
            if (list == null || list.Count == 0) return false;

            foreach (var record in list)
            {
                if (record != null && record.IsValid && record.Target == target)
                {
                    record.SetInValid();
                }
            }

            return true;
        }

        public bool RemoveByTarget(object target)
        {
            if (target == null) return false;

            foreach (var (evetName, eventRecordList) in _eventMap)
            {
                foreach (var record in eventRecordList)
                {
                    if (record.Target == target)
                    {
                        record.SetInValid();
                    }
                }
            }
            
            return true;
        }

        public void CheckInvalidEventRecord()
        {
            foreach (var (eventName, records) in _eventMap)
            {
                for (int i = records.Count - 1; i >= 0; i--)
                {
                    var record = records[i];
                    if (record != null && !record.IsValid)
                    {
                        records.RemoveAt(i);
                        RecyclePool<EventRecord>.Default.Recycle(record);
                    }
                }

                if (records.Count == 0)
                {
                    ReleaseList2Pool(records);
                }
            }
        }

        #region EventMap
        
        private List<EventRecord> GetRecordList(string eventName)
        {
            return _eventMap.GetValueOrDefault(eventName);
        }
        
        private void AddEventList(string eventName, List<EventRecord> list)
        {
            _eventMap[eventName] = list;
        }

        #endregion
        
        #region List<EventRecord> Pool

        private List<EventRecord> GetListFromPool()
        {
            return new List<EventRecord>();
        }

        private void ReleaseList2Pool(List<EventRecord> list)
        {
            list = null;
        }

        #endregion
    }
}