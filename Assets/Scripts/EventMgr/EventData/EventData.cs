using Better;

namespace Game.EventMgr
{
    public abstract class EventDataBase : IRecycle
    {
        public bool IsRecycled { get; set; }
        public virtual void Recycle()
        {
            
        }
    }

    public class EventData : EventDataBase
    {
    }

    public class EventData<T> : EventDataBase
    {
        public T Data1 { get; private set; }

        public override void Recycle()
        {
            Data1 = default;
        }

        public void SetData(T data1)
        {
            Data1 = data1;
        }
    }
    
    public class EventData<T1, T2> : EventDataBase
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }

        public override void Recycle()
        {
            Data1 = default;
            Data2 = default;
        }

        public void SetData(T1 data1, T2 data2)
        {
            Data1 = data1;
            Data2 = data2;
        }
    }
    
    public class EventData<T1, T2, T3> : EventDataBase
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }
        public T3 Data3 { get; private set; }

        public override void Recycle()
        {
            Data1 = default;
            Data2 = default;
            Data3 = default;
        }

        public void SetData(T1 data1, T2 data2, T3 data3)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
        }
    }
    
    public class EventData<T1, T2, T3, T4> : EventDataBase
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }
        public T3 Data3 { get; private set; }
        public T4 Data4 { get; private set; }

        public override void Recycle()
        {
            Data1 = default;
            Data2 = default;
            Data3 = default;
            Data4 = default;
        }

        public void SetData(T1 data1, T2 data2, T3 data3, T4 data4)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;
        }
    }
    
    public class EventData<T1, T2, T3, T4, T5> : EventDataBase
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }
        public T3 Data3 { get; private set; }
        public T4 Data4 { get; private set; }
        public T5 Data5 { get; private set; }

        public override void Recycle()
        {
            Data1 = default;
            Data2 = default;
            Data3 = default;
            Data4 = default;
            Data5 = default;
        }

        public void SetData(T1 data1, T2 data2, T3 data3, T4 data4, T5 data5)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;
            Data5 = data5;
        }
    }
}