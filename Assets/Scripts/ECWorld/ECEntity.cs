using System.Collections.Generic;
using UnityEngine;

namespace Game.ECWorld
{
    public class ECEntity : ECObjet, IECRecycled
    {
        private int _insId;
        public int InsID
        {
            get => _insId;
            set
            {
                if (value != 0) return; // 已经有值了，不允许再设置
                _insId = value;
            }
        }

        private List<ECComponent> _comps = new(); // 按照 ECComp.TypeId 从小到大排
        private List<ECComponent> _pendingStartComps = new();

        public void AddComponent(ECComponent comp)
        {
            if (comp == null) return;

            if (comp.Entity != null)
            {
                Debug.LogError($"ECComponent(typeId={comp.TypeId}) already has entity");
                return;
            }

            var index = _comps.Count;
            for (int i = 0; i < _comps.Count; i++)
            {
                if (comp.TypeId < _comps[i].TypeId)
                {
                    index = i;
                    break;
                }
                else if (comp.TypeId == _comps[i].TypeId)
                {
                    Debug.LogError($"ECComponent(typeId={comp.TypeId}) already exists");
                    return;
                }
            }

            comp.Entity = this;
            _comps.Insert(index, comp);

            if (_isAwakened)
            {
                comp.Awake();
            }

            if (_isStarted)
            {
                index = _pendingStartComps.Count;
                for (int i = 0; i < _pendingStartComps.Count; i++)
                {
                    if (comp.TypeId >= _pendingStartComps[i].TypeId)
                    {
                        index = i;
                        break;
                    }
                }
                
                _pendingStartComps.Insert(index, comp);
            }
        }

        public void RemoveComponent(ECComponent comp)
        {
            if (comp == null) return;

            var index = -1;
            for (int i = 0; i < _comps.Count; i++)
            {
                if( comp == _comps[i])
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Debug.LogError($"ECComponent(typeId={comp.TypeId}) not found");
                return;
            }

            if (_pendingStartComps.Count > 0 && _pendingStartComps.Contains(comp))
            {
                _pendingStartComps.Remove(comp);
            }
            _comps.RemoveAt(index);
            comp.Destroy();
        }

        public ECComponent GetComponent(int typeId)
        {
            foreach (var comp in _comps)
            {
                if (comp.TypeId == typeId)
                {
                    return comp;
                }
            }

            return null;
        }

        protected override void OnAwake()
        {
            foreach (var comp in _comps)
            {
                comp.Awake();
            }
        }

        protected override void OnStart()
        {
            foreach (var comp in _comps)
            {
                comp.Start();
            }
        }

        protected override void OnDestroy()
        {
            for (int i = _comps.Count; i >= 0 ; i--)
            {
                _comps[i].Destroy();
            }
        }

        protected override void OnUpdate()
        {
            InnerCheck();
            foreach (var comp in _comps)
            {
                if (_enabled && comp.Enabled && comp.RequiredUpdate)
                {
                    comp.Update();
                }
            }
        }

        protected override void OnLateUpdate()
        {
            InnerCheck();
            foreach (var comp in _comps)
            {
                if (_enabled && comp.Enabled && comp.RequiredLateUpdate)
                {
                    comp.LateUpdate();
                }
            }
        }

        protected override void OnFixedUpdate()
        {
            foreach (var comp in _comps)
            {
                if (_enabled && comp.Enabled && comp.RequiredFixedUpdate)
                {
                    comp.FixedUpdate();
                }
            }
        }

        protected override void OnAnimationJobRunning()
        {
            InnerCheck();
            foreach (var comp in _comps)
            {
                if (_enabled && comp.Enabled && comp.RequiredAnimationJobRunning)
                {
                    comp.AnimationJobRunning();
                }
            }
        }

        protected override void OnPhysicalJobRunning()
        {
            InnerCheck();
            foreach (var comp in _comps)
            {
                if (_enabled && comp.Enabled && comp.RequiredPhysicsJobRunning)
                {
                    comp.PhysicalJobRunning();
                }
            }
        }

        private void InnerCheck()
        {
            var cnt = _pendingStartComps.Count;
            if (cnt == 0) return;

            for (int i = cnt - 1; i >= 0; i--)
            {
                var comp = _pendingStartComps[i];
                if (comp != null && _enabled)
                {
                    _pendingStartComps.RemoveAt(i);
                    comp.Start();
                }
            }
        }
        
        public bool IsRecycled { get; set; }
        public void Recycle()
        {
            _enabled = false;
            _isAwakened = false;
            _isStarted = false;
            _isDestroyed = false;
        }
    }
}