using System.Collections.Generic;

namespace Better
{
    public interface IRecycle
    {
        public bool IsRecycled { get; set; }
        
        public void Recycle();
    }
    
    public abstract class RecyclePool
    {
        public abstract IRecycle GetObject();
        
        public abstract void RecycleObject(IRecycle item);
    }

    public class RecyclePool<T> : RecyclePool where T : IRecycle, new()
    {
        const int DEFAULT_INIT_NUM = 8;
        const int DEFAULT_STEP_NUM = 8;
        
        private static RecyclePool<T> _default;
        public static RecyclePool<T> Default => _default ??= new RecyclePool<T>();
        
        private readonly Stack<T> _stack = new Stack<T>();
        
        private int _stepNum;

        public RecyclePool(int initNum, int stepNum)
        {
            for (int i = 0; i < initNum; i++)
            {
                _stack.Push(new T() { IsRecycled = true });
            }
            
            _stepNum = stepNum;
        }

        public RecyclePool() : this(DEFAULT_INIT_NUM, DEFAULT_STEP_NUM)
        {
            
        }

        public override IRecycle GetObject() => Get();

        public T Get()
        {
            if (_stack.Count > 0)
            {
                var item = _stack.Pop();
                item.IsRecycled = false;
                return _stack.Pop();
            }

#if !BAMBOO_POOL_RELEASE && (BAMBOO_POOL_DEBUG || UNITY_EDITOR)
            UnityEngine.Debug.Log($"[POOL] RecyclePool<{typeof(T)}> new generation {_stepNum} item.");
#endif

            for (int i = 1; i < _stepNum; i++)
            {
                _stack.Push(new T() { IsRecycled = true });
            }

            return new T() { IsRecycled = false };
        }

        public void Recycle(T item)
        {
            if (item == null) return;
            if (item.IsRecycled) return;

            item.Recycle();
            item.IsRecycled = true;

#if !BAMBOO_POOL_RELEASE && (BAMBOO_POOL_DEBUG || UNITY_EDITOR)
            if (_stack.Contains(item))
            {
                UnityEngine.Debug.LogError($"{item} is already in the RecyclePool<{typeof(T)}>.");
            }
#endif

            _stack.Push(item);
        }

        public override void RecycleObject(IRecycle item)
        {
            Recycle((T)item);
        }
    }
}