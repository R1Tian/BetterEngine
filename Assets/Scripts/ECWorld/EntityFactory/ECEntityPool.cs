using System.Collections.Generic;

namespace Game.ECWorld
{
    public abstract class ECEntityPool
    {
        public abstract ECEntity GetEntity();
        public abstract void RecycleEntity(ECEntity entity);
    }

    public class ECEntityPool<T> : ECEntityPool where T : ECEntity, new()
    {
        private Stack<ECEntity> _stack;
        private int _stepNum;

        private ECEntityFactory _factory;

        public ECEntityPool(ECEntityFactory factory, int stepNum = 8)
        {
            _stack = new Stack<ECEntity>();
            _stepNum = stepNum < 1 ? 1 : stepNum;
        }

        public override ECEntity GetEntity()
        {
            if (_stack.Count > 0)
            {
                var entity = _stack.Pop();
                entity.IsRecycled = false;
                return entity;
            }

            for (int i = 0; i < _stepNum; i++)
            {
                var entity = new T
                {
                    InsID = _factory.GetInsID(),
                    IsRecycled = true
                };
                _stack.Push(entity);
            }

            var pop = _stack.Pop();
            pop.IsRecycled = false;
            return pop;
        }

        public override void RecycleEntity(ECEntity entity)
        {
           if(entity == null) return;
           if(entity.IsRecycled) return;
           
           entity.Recycle();
           entity.IsRecycled = true;
           
           _stack.Push(entity);
        }
    }

    public interface IECRecycled
    {
        public bool IsRecycled { get; set; }
        
        public void Recycle();
    }
}