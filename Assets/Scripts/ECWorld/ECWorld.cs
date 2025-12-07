using System.Collections.Generic;

namespace Game.ECWorld
{
    public class ECWorld : ECObjet
    {
        private float _deltaTime = 0;
        private float _realTime = 0;
        private float _frameCount = 0;
        private int _iterateIndex = 0;

        private List<ECEntity> _entities = new();
        private List<ECEntity> _pendingStartEntities = new();

        private ECEntityFactory _factory = new();

        public T NewEntity<T>() where T : ECEntity, new()
        {
            return _factory.GetEntity<T>();
        }

        public void AddEntity(ECEntity entity)
        {
            if (entity == null) return;
            if (GetEntity(entity.InsID) != null) return;

            _entities.Add(entity);
            if (_isAwakened)
            {
                entity.Awake();
            }
            if (_isStarted)
            {
                _pendingStartEntities.Add(entity);
            }
        }

        public ECEntity GetEntity(int indId)
        {
            foreach (var entity in _entities)
            {
                if (entity.InsID == indId)
                {
                    return entity;
                }
            }
            return null;
        }

        // 替换ECObject.Update，因为要多一个 deltaTime 参数
        // 考虑是否 ECObject.Update 就带一个 deltaTime 参数
        public void UpdateByDriver(float deltaTime)
        {
            if(!CanTick()) return;

            _frameCount += 1;
            _deltaTime = deltaTime;
            _realTime += deltaTime;
            Update();
        }

        public void DestroyByDriver()
        {
            Destroy();
            _frameCount = 0;
            _deltaTime = 0;
            _realTime = 0;
            _entities.Clear();
            _pendingStartEntities.Clear();
        }

        protected override void OnAwake()
        {
            Tick(LifecycleFuncType.Awake);
        }

        protected override void OnStart()
        {
            Tick(LifecycleFuncType.Start);
        }

        protected override void OnUpdate()
        {
            InnerCheck();
            Tick(LifecycleFuncType.Update);
        }

        protected override void OnAnimationJobRunning()
        {
            InnerCheck();
            Tick(LifecycleFuncType.AnimationJobRunning);
        }

        protected override void OnLateUpdate()
        {
            InnerCheck();
            Tick(LifecycleFuncType.LateUpdate);
        }

        protected override void OnPhysicalJobRunning()
        {
            InnerCheck();
            Tick(LifecycleFuncType.PhysicalJobRunning);
        }

        protected override void OnFixedUpdate()
        {
            Tick(LifecycleFuncType.FixedUpdate);
        }

        protected override void OnDestroy()
        {
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                var entity = _entities[i];
                if (entity == null) continue;
                entity.Destroy();
            }
        }

        private void Tick(LifecycleFuncType type)
        {
            _iterateIndex = 1;
            while (_iterateIndex <= _entities.Count)
            {
                var entity = _entities[_iterateIndex];
                switch (type)
                {
                    case LifecycleFuncType.Awake: entity.Awake(); break;
                    case LifecycleFuncType.Start: entity.Start(); break;
                    case LifecycleFuncType.Update: entity.Update(); break;
                    case LifecycleFuncType.FixedUpdate: entity.FixedUpdate(); break;
                    case LifecycleFuncType.LateUpdate: entity.LateUpdate(); break;
                    case LifecycleFuncType.AnimationJobRunning: entity.AnimationJobRunning(); break;
                    case LifecycleFuncType.PhysicalJobRunning: entity.PhysicalJobRunning(); break;
                }
                entity.Update();
                _iterateIndex++;
            }
            _iterateIndex = 0;
        }

        private void InnerCheck()
        {
            var cnt = _pendingStartEntities.Count;
            if(cnt == 0) return;
            for (int i = cnt - 1; i >= 0; i--)
            {
                var entity = _pendingStartEntities[i];
                if (entity != null && _enabled)
                {
                    _pendingStartEntities.Remove(entity);
                    entity.Start();
                }
            }
        }
    }
}