using Better.Collections;
using System;

namespace Better.Chronos
{
    public struct Tweener
    {
        internal int Index;
        internal int Version;
        internal EventTween EventTween;

        internal Tweener(int index, int version, EventTween eventTween)
        {
            Index = index;
            Version = version;
            EventTween = eventTween;
        }
    }

    public static class TweenerExtension
    {
        public static bool IsValid(this in Tweener tweener)
        {
            var eventTween = tweener.EventTween;
            if (null == eventTween) return false;

            return eventTween.IsValid(tweener.Index, tweener.Version);
        }

        public static void Cancel(this in Tweener tweener)
        {
            var eventTween = tweener.EventTween;
            if (null == eventTween) return;

            eventTween.Cancel(tweener.Index, tweener.Version);
        }

        public static void Pause(this in Tweener tweener)
        {
            var eventTween = tweener.EventTween;
            if (null == eventTween) return;

            eventTween.Pause(tweener.Index, tweener.Version);
        }

        public static void Resume(this in Tweener tweener)
        {
            var eventTween = tweener.EventTween;
            if (null == eventTween) return;

            eventTween.Resume(tweener.Index, tweener.Version);
        }
    }

    public class EventTween
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

        public EventTween()
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

        public Tweener Add(int duration, Action<int> onUpdate, Action<bool> onComplete)
        {
            var info = _infoPool.Get();
            info.Duration = duration;
            info.IsPause = _isPause;

            var updateActionInfo = _actionInfoPools.Get<UpdateActionInfo>();
            updateActionInfo.Action = onUpdate;
            info.UpdateActionInfo = updateActionInfo;

            var completeActionInfo = _actionInfoPools.Get<CompleteActionInfo>();
            completeActionInfo.Action = onComplete;
            info.CompleteActionInfo = completeActionInfo;

            return InnerAdd(info);
        }

        public Tweener Add<T>(int duration, Action<T, int> onUpdate, Action<T, bool> onComplete, T arg)
        {
            var info = _infoPool.Get();
            info.Duration = duration;
            info.IsPause = _isPause;

            var updateActionInfo = _actionInfoPools.Get<UpdateActionInfo<T>>();
            updateActionInfo.Action = onUpdate;
            updateActionInfo.Arg = arg;
            info.UpdateActionInfo = updateActionInfo;

            var completeActionInfo = _actionInfoPools.Get<CompleteActionInfo<T>>();
            completeActionInfo.Action = onComplete;
            completeActionInfo.Arg = arg;
            info.CompleteActionInfo = completeActionInfo;

            return InnerAdd(info);
        }

        public Tweener Add<T1, T2>(int duration, Action<T1, T2, int> onUpdate, Action<T1, T2, bool> onComplete, T1 arg1, T2 arg2)
        {
            var info = _infoPool.Get();
            info.Duration = duration;
            info.IsPause = _isPause;

            var updateActionInfo = _actionInfoPools.Get<UpdateActionInfo<T1, T2>>();
            updateActionInfo.Action = onUpdate;
            updateActionInfo.Arg1 = arg1;
            updateActionInfo.Arg2 = arg2;
            info.UpdateActionInfo = updateActionInfo;

            var completeActionInfo = _actionInfoPools.Get<CompleteActionInfo<T1, T2>>();
            completeActionInfo.Action = onComplete;
            completeActionInfo.Arg1 = arg1;
            completeActionInfo.Arg2 = arg2;
            info.CompleteActionInfo = completeActionInfo;

            return InnerAdd(info);
        }

        public Tweener Add<T1, T2, T3>(int duration, Action<T1, T2, T3, int> onUpdate, Action<T1, T2, T3, bool> onComplete, T1 arg1, T2 arg2, T3 arg3)
        {
            var info = _infoPool.Get();
            info.Duration = duration;
            info.IsPause = _isPause;

            var updateActionInfo = _actionInfoPools.Get<UpdateActionInfo<T1, T2, T3>>();
            updateActionInfo.Action = onUpdate;
            updateActionInfo.Arg1 = arg1;
            updateActionInfo.Arg2 = arg2;
            updateActionInfo.Arg3 = arg3;
            info.UpdateActionInfo = updateActionInfo;

            var completeActionInfo = _actionInfoPools.Get<CompleteActionInfo<T1, T2, T3>>();
            completeActionInfo.Action = onComplete;
            completeActionInfo.Arg1 = arg1;
            completeActionInfo.Arg2 = arg2;
            completeActionInfo.Arg3 = arg3;
            info.CompleteActionInfo = completeActionInfo;

            return InnerAdd(info);
        }

        public bool IsValid(int index, int version)
        {
            return _exists[index] && _versions[index] == version;
        }

        public void Cancel(int index, int version)
        {
            InnerRemoveToQueue(index, version);
        }

        public void Pause(int index, int version)
        {
            InnerPauseToQueue(index, version);
        }

        public void Resume(int index, int version)
        {
            InnerResumeToQueue(index, version);
        }

        public void CancelAll()
        {
            _count = 0;

            int len = _infos.Length;
            for (int i = 0; i < len; ++i)
            {
                var info = _infos[i];
                if (null == info) continue;

                _actionInfoPools.Recycle(info.UpdateActionInfo);
                _actionInfoPools.Recycle(info.CompleteActionInfo);
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

        public void PauseAll()
        {
            _isPause = true;
        }

        public void ResumeAll()
        {
            _isPause = false;
        }

        public void Update(int delta)
        {
            ExecuteOperateQueue();

            if (_isPause) return;

            int len = _infos.Length;
            for (int i = 0; i < len; ++i)
            {
                var info = _infos[i];
                if (null == info) continue;

                if (info.IsPause) continue;

                if (info.Elapsed >= info.Duration)
                {
                    info.UpdateActionInfo.Execute(1000);
                    info.CompleteActionInfo.Execute(false);

                    _actionInfoPools.Recycle(info.UpdateActionInfo);
                    _actionInfoPools.Recycle(info.CompleteActionInfo);
                    _infoPool.Recycle(info);

                    _infos[i] = null;
                    _exists[i] = false;
                    ++_versions[i];

                    --_count;
                }
                else
                {
                    info.UpdateActionInfo.Execute(1000 * info.Elapsed / info.Duration); 
                    info.Elapsed += delta;
                }
            }
        }

        Tweener InnerAdd(Info info)
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

                return new Tweener(i, _versions[i], this);
            }

            for (int i = 0; i < index; ++i)
            {
                if (_exists[i]) continue;

                _exists[i] = true;
                ++_versions[i];

                _lastIndex = i;

                InnerAddToQueue(i, _versions[i], info);

                return new Tweener(i, _versions[i], this);
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
                            _infos[opIndex].CompleteActionInfo.Execute(true);

                            _exists[opIndex] = false;
                            ++_versions[opIndex];

                            _actionInfoPools.Recycle(_infos[opIndex].UpdateActionInfo);
                            _actionInfoPools.Recycle(_infos[opIndex].CompleteActionInfo);
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

        internal enum OperateType
        {
            Add = 0,
            Remove = 1,
            Pause = 2,
            Resume = 3,
        }

        internal struct OperateItem
        {
            public OperateType OperateType;
            public int Index;
            public int Version;
            public Info Info;
        }

        internal sealed class Info : IRecycle
        {
            public int Duration;
            public int Elapsed;
            public bool IsPause;
            public BaseUpdateActionInfo UpdateActionInfo;
            public BaseCompleteActionInfo CompleteActionInfo;

            public bool IsRecycled { get; set; }

            public void Recycle()
            {
                Duration = 0;
                Elapsed = 0;
                IsPause = false;
                UpdateActionInfo = null;
                CompleteActionInfo = null;
            }
        }
    }
}