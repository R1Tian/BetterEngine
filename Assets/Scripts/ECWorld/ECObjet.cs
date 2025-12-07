namespace Game.ECWorld
{
    public class ECObjet
    {
        protected bool _enabled = false;
        protected bool _isAwakened = false;
        protected bool _isStarted = false;
        protected bool _isDestroyed = false;
        
        public bool Enabled => _enabled;

        public void Awake()
        {
            if (_isAwakened) return;

            _enabled = true;
            OnAwake();
            _isAwakened = true;
        }
        
        public void Start()
        {
            if (_isStarted || !_isAwakened) return;
            
            OnStart();
            _isStarted = true;
        }

        public void Destroy()
        {
            if (_isDestroyed) return;

            _enabled = false;
            OnDestroy();
            _isDestroyed = true;
        }

        public void Update()
        {
            if(!CanTick()) return;
            
            OnUpdate();
        }

        public void LateUpdate()
        {
            if(!CanTick()) return;
            
            OnLateUpdate();
        }
        
        public void FixedUpdate()
        {
            if(!CanTick()) return;
            
            OnFixedUpdate();
        }

        public void AnimationJobRunning()
        {
            if(!CanTick()) return;
            
            OnAnimationJobRunning();
        }

        public void PhysicalJobRunning()
        {
            if(!CanTick()) return;
            
            OnPhysicalJobRunning();
        }

        protected virtual void OnAwake()
        {
            
        }

        protected virtual void OnStart()
        {
            
        }

        protected virtual void OnDestroy()
        {
            
        }

        protected virtual void OnUpdate()
        {
            
        }

        protected virtual void OnLateUpdate()
        {
            
        }

        protected virtual void OnFixedUpdate()
        {
            
        }

        protected virtual void OnAnimationJobRunning()
        {
            
        }

        protected virtual void OnPhysicalJobRunning()
        {
            
        }

        protected bool CanTick()
        {
            return _enabled && _isAwakened && _isStarted && !_isDestroyed;
        }
    }
}