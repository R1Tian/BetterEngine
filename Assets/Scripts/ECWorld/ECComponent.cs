namespace Game.ECWorld
{
    public class ECComponent : ECObjet
    {
        private int _typeId;
        private bool _requiredUpdate;
        private bool _requiredLateUpdate;
        private bool _requiredFixedUpdate;
        private bool _requiredAnimationJobRunning;
        private bool _requiredPhysicsJobRunning;
        
        public bool RequiredUpdate => _requiredUpdate;
        public bool RequiredLateUpdate => _requiredLateUpdate;
        public bool RequiredFixedUpdate => _requiredFixedUpdate;
        public bool RequiredAnimationJobRunning => _requiredAnimationJobRunning;
        public bool RequiredPhysicsJobRunning => _requiredPhysicsJobRunning;

        public int TypeId => _typeId;
        public ECEntity Entity { get; set; }

        public ECComponent(int typeId) : base()
        {
            _typeId = typeId;
            _requiredUpdate = true;
            _requiredLateUpdate = false;
            _requiredFixedUpdate = false;
            _requiredAnimationJobRunning = false;
            _requiredPhysicsJobRunning = false;
        }
    }
}