using System;
using Better.Collections;

namespace Better.Chronos
{
    public struct Delayer
    {
        internal int Index;
        internal int Version;
        internal EventDelay EventDelay;

        internal Delayer(int index, int version, EventDelay eventDelay)
        {
            Index = index;
            Version = version;
            EventDelay = eventDelay;
        }
    }
    
    public static class DelayerExtension
    {
        public static bool IsValid(this in Delayer delayer)
        {
            var eventDelay = delayer.EventDelay;
            if (null == eventDelay) return false;

            return eventDelay.IsValid(delayer.Index, delayer.Version);
        }

        public static void Cancel(this in Delayer delayer)
        {
            var eventDelay = delayer.EventDelay;
            if (null == eventDelay) return;

            eventDelay.Cancel(delayer.Index, delayer.Version);
        }

        public static void Pause(this in Delayer delayer)
        {
            var eventDelay = delayer.EventDelay;
            if (null == eventDelay) return;

            eventDelay.Pause(delayer.Index, delayer.Version);
        }

        public static void Resume(this in Delayer delayer)
        {
            var eventDelay = delayer.EventDelay;
            if (null == eventDelay) return;

            eventDelay.Resume(delayer.Index, delayer.Version);
        }
    }
    
    internal class EventDelay
    {
        const int MIN_LENGTH = 8;

        readonly BArray<bool> _exists;
        readonly BArray<int> _versions;
        readonly BArray<Info> _infos;
        readonly RecyclePool<Info> _infoPool;
        readonly ActionInfoPools _actionInfoPools;

        BArray<OperateItem> _operateItems;

        bool _isPause;
        int _count;
        int _lastIndex;

        internal EventDelay()
        {
            _exists = BArrayPool<bool>.Default.Get(MIN_LENGTH);
            _versions = BArrayPool<int>.Default.Get(MIN_LENGTH);
            _infos = BArrayPool<Info>.Default.Get(MIN_LENGTH);
            _infoPool = new RecyclePool<Info>();
            _actionInfoPools = new ActionInfoPools();
            
            _isPause = false;
            _count = 0;
            _lastIndex = -1;
        }
        
        public bool IsValid(int index, int version)
        {
            return _exists[index] && _versions[index] == version;
        }
        
        #region Add
        public Delayer Add(int delay, Action action)
        {
            var info = _infoPool.Get();
            info.Delay = delay;
            info.IsPause = _isPause;

            var actionInfo = _actionInfoPools.Get<ActionInfo>();
            actionInfo.Action = action;
            info.ActionInfo = actionInfo;

            return InnerAdd(info);
        }

        public Delayer Add<T>(int delay, Action<T> action, T arg)
        {
            var info = _infoPool.Get();
            info.Delay = delay;
            info.IsPause = _isPause;
            
            var actionInfo = _actionInfoPools.Get<ActionInfo<T>>();
            actionInfo.Action = action;
            actionInfo.Arg = arg;
            info.ActionInfo = actionInfo;
            
            return InnerAdd(info);
        }
        
        public Delayer Add<T1, T2>(int delay, Action<T1,T2> action, T1 arg1, T2 arg2)
        {
            var info = _infoPool.Get();
            info.Delay = delay;
            info.IsPause = _isPause;
            
            var actionInfo = _actionInfoPools.Get<ActionInfo<T1, T2>>();
            actionInfo.Action = action;
            actionInfo.Arg1 = arg1;
            actionInfo.Arg2 = arg2;
            info.ActionInfo = actionInfo;
            
            return InnerAdd(info);
        }
        
        public Delayer Add<T1, T2, T3>(int delay, Action<T1,T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            var info = _infoPool.Get();
            info.Delay = delay;
            info.IsPause = _isPause;
            
            var actionInfo = _actionInfoPools.Get<ActionInfo<T1, T2, T3>>();
            actionInfo.Action = action;
            actionInfo.Arg1 = arg1;
            actionInfo.Arg2 = arg2;
            actionInfo.Arg3 = arg3;
            info.ActionInfo = actionInfo;
            
            return InnerAdd(info);
        }
        
        Delayer InnerAdd(Info info)
        {
            ++_count;
            int length = _exists.Length;
            if (_count + (_count >> 1) > length)
            {
                length *= 2;
                _exists.ResetLength(length);
                _versions.ResetLength(length);
            }

            int index = _lastIndex + 1;

            for (int i = index; i < length; ++i)
            {
                if (_exists[i]) continue;

                _exists[i] = true;
                ++_versions[i];

                _lastIndex = i;

                InnerAddToQueue(i, _versions[i], info);

                return new Delayer(i, _versions[i], this);
            }

            for (int i = 0; i < index; ++i)
            {
                if (_exists[i]) continue;

                _exists[i] = true;
                ++_versions[i];

                _lastIndex = i;

                InnerAddToQueue(i, _versions[i], info);

                return new Delayer(i, _versions[i], this);
            }

            throw new System.Exception($"Impossible Exception." +
                                       $" _count : {_count}" +
                                       $" _exists.Length : {_exists.Length}" +
                                       $" _exists.Capacity : {_exists.Capacity}" +
                                       $" _lastIndex : {_lastIndex}");
        }

        void InnerAddToQueue(int index, int version, Info info)
        {
            if (null == _operateItems)
            {
                _operateItems = BArrayPool<OperateItem>.Default.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Add,
                    Index = index,
                    Version = version,
                    Info = info
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Add,
                    Index = index,
                    Version = version,
                    Info = info
                };
            }
        }
        
        #endregion
        
        #region Cancel
        public void Cancel(int index, int version)
        {
            InnerRemoveToQueue(index, version);
        }

        void InnerRemoveToQueue(int index, int version)
        {
            if (null == _operateItems)
            {
                _operateItems = BArrayPool<OperateItem>.Default.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Remove,
                    Index = index,
                    Version = version
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Remove,
                    Index = index,
                    Version = version
                };
            }
        }

        public void CancelAll()
        {
            _count = 0;

            int len = _infos.Length;
            for (int i = 0; i < len; ++i)
            {
                var info = _infos[i];
                if (null == info) continue;

                _actionInfoPools.Recycle(info.ActionInfo);
                _infoPool.Recycle(info);

                _infos[i] = null;
            }

            len = _exists.Length;
            for (int i = 0; i < len; ++i)
            {
                _exists[i] = false;
            }

            BArrayPool<OperateItem>.Default.Recycle(_operateItems);
            _operateItems = null;
        }
        
        #endregion

        #region Pause
        public void Pause(int index, int version)
        {
            InnerPauseToQueue(index, version);
        }
        
        void InnerPauseToQueue(int index, int version)
        {
            if (null == _operateItems)
            {
                _operateItems = BArrayPool<OperateItem>.Default.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Pause,
                    Index = index,
                    Version = version
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Pause,
                    Index = index,
                    Version = version
                };
            }
        }
        
        public void PauseAll()
        {
            _isPause = true;
        }
        
        #endregion

        #region Resume
        public void Resume(int index, int version)
        {
            InnerResumeToQueue(index, version);
        }

        void InnerResumeToQueue(int index, int version)
        {
            if (null == _operateItems)
            {
                _operateItems = BArrayPool<OperateItem>.Default.Get(1);

                _operateItems[0] = new OperateItem()
                {
                    OperateType = OperateType.Resume,
                    Index = index,
                    Version = version
                };
            }
            else
            {
                int len = _operateItems.Length;
                _operateItems.ResetLength(len + 1);

                _operateItems[len] = new OperateItem()
                {
                    OperateType = OperateType.Resume,
                    Index = index,
                    Version = version
                };
            }
        }
        
        public void ResumeAll()
        {
            _isPause = false;
        }
        
        #endregion

        void ExecuteOperateQueue()
        {
            if (null == _operateItems) return;

            int len = _operateItems.Length;
            for (int i = 0; i < len; ++i)
            {
                var opItem = _operateItems[i];
                int opIndex = opItem.Index;

                switch (opItem.OperateType)
                {
                    case OperateType.Add:
                        if (_infos.Length < _exists.Length)
                        {
                            _infos.ResetLength(_exists.Length);
                        }
                        _infos[opIndex] = opItem.Info;

                        break;
                    case OperateType.Remove:
                        if (_exists[opIndex] && _versions[opIndex] == opItem.Version)
                        {
                            _exists[opIndex] = false;
                            ++_versions[opIndex];

                            _actionInfoPools.Recycle(_infos[opIndex].ActionInfo);
                            _infoPool.Recycle(_infos[opIndex]);
                            _infos[opIndex] = null;

                            --_count;
                        }

                        break;
                    case OperateType.Pause:
                        if (_exists[opIndex] && _versions[opIndex] == opItem.Version)
                        {
                            _infos[opIndex].IsPause = true;
                        }

                        break;
                    case OperateType.Resume:
                        if (_exists[opIndex] && _versions[opIndex] == opItem.Version)
                        {
                            _infos[opIndex].IsPause = false;
                        }

                        break;
                }
            }

            BArrayPool<OperateItem>.Default.Recycle(_operateItems);
            _operateItems = null;
        }

        public void Update(int delta)
        {
            ExecuteOperateQueue();
            
            if (_isPause) return;

            int len = _infos.Length;
            for (int i = 0; i < len; ++i)
            {
                var info = _infos[i];
                if (info == null) continue;

                if (info.IsPause) continue;

                if (info.Delay <= 0)
                {
                    info.ActionInfo.Execute();

                    _actionInfoPools.Recycle(info.ActionInfo);
                    _infoPool.Recycle(info);

                    _infos[i] = null;
                    _exists[i] = false;
                    ++_versions[i];

                    --_count;
                }
                else
                {
                    info.Delay -= delta;
                }
            }
        }
        
        internal enum OperateType
        {
            Add = 0,
            Remove = 1,
            Pause = 2,
            Resume = 3,
        }

        private struct OperateItem
        {
            public OperateType OperateType;
            public int Index;
            public int Version;
            public Info Info;
        }

        private sealed class Info : IRecycle
        {
            public int Delay;
            public bool IsPause;
            public BaseActionInfo ActionInfo;

            public bool IsRecycled { get; set; }

            public void Recycle()
            {
                Delay = 0;
                IsPause = false;
                ActionInfo = null;
            }
        }
    }
}