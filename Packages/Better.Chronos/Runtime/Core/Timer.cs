using System;

namespace Better.Chronos
{
    public class Timer : IDisposable
    {
        readonly HeartBeat _timeBeat;
        readonly HeartBeat _frameBeat;
        readonly HeartTime _heartTime;
        
        private EventDelay _timeDelay;
        private EventDelay _frameDelay;
        private EventLoop _timeLoop;
        private EventLoop _frameLoop;
        private EventTween _timeTween;
        private EventTween _frameTween;

        public Timer(HeartBeat timeBeat, HeartBeat frameBeat)
        {
            _timeBeat = timeBeat;
            _frameBeat = frameBeat;
            _heartTime = new HeartTime(timeBeat, frameBeat);
        }
        
        public void Dispose()
        {
            if (null != _timeDelay && null != _timeBeat) _timeBeat.Remove(_timeDelay.Update);
            if (null != _frameDelay && null != _frameBeat) _frameBeat.Remove(_frameDelay.Update);
            if (null != _timeLoop && null != _timeBeat) _timeBeat.Remove(_timeLoop.Update);
            if (null != _frameLoop && null != _frameBeat) _frameBeat.Remove(_frameLoop.Update);
            if (null != _timeTween && null != _timeBeat) _timeBeat.Remove(_timeTween.Update);
            if (null != _frameTween && null != _frameBeat) _frameBeat.Remove(_frameTween.Update);

            _timeDelay = null;
            _frameDelay = null;
            _timeLoop = null;
            _frameLoop = null;
            _timeTween = null;
            _frameTween = null;
        }

        #region Delay
        public Delayer DelayMs(int delayMs, Action action)
        {
            if (null == _timeDelay)
            {
                _timeDelay = new EventDelay();
                _timeBeat.Add(_timeDelay.Update);
            }

            return _timeDelay.Add(delayMs, action);
        }

        public Delayer DelayMs<T>(int delayMs, Action<T> action, T arg)
        {
            if (null == _timeDelay)
            {
                _timeDelay = new EventDelay();
                _timeBeat.Add(_timeDelay.Update);
            }

            return _timeDelay.Add(delayMs, action, arg);
        }
        
        public Delayer DelayMs<T1, T2>(int delayMs, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (null == _timeDelay)
            {
                _timeDelay = new EventDelay();
                _timeBeat.Add(_timeDelay.Update);
            }

            return _timeDelay.Add(delayMs, action, arg1, arg2);
        }
        
        public Delayer DelayMs<T1, T2, T3>(int delayMs, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (null == _timeDelay)
            {
                _timeDelay = new EventDelay();
                _timeBeat.Add(_timeDelay.Update);
            }

            return _timeDelay.Add(delayMs, action, arg1, arg2, arg3);
        }
        
        public Delayer DelayFrame(int delayFrame, Action action)
        {
            if (null == _frameDelay)
            {
                _frameDelay = new EventDelay();
                _frameBeat.Add(_frameDelay.Update);
            }

            return _frameDelay.Add(delayFrame, action);
        }

        public Delayer DelayFrame<T>(int delayFrame, Action<T> action, T arg)
        {
            if (null == _frameDelay)
            {
                _frameDelay = new EventDelay();
                _frameBeat.Add(_frameDelay.Update);
            }

            return _frameDelay.Add(delayFrame, action, arg);
        }

        public Delayer DelayFrame<T1, T2>(int delayFrame, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (null == _frameDelay)
            {
                _frameDelay = new EventDelay();
                _frameBeat.Add(_frameDelay.Update);
            }

            return _frameDelay.Add(delayFrame, action, arg1, arg2);
        }

        public Delayer DelayFrame<T1, T2, T3>(int delayFrame, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (null == _frameDelay)
            {
                _frameDelay = new EventDelay();
                _frameBeat.Add(_frameDelay.Update);
            }

            return _frameDelay.Add(delayFrame, action, arg1, arg2, arg3);
        }
        
        #endregion

        #region Loop
        public Looper LoopMs(int intervalMs, Action action)
        {
            if (null == _timeLoop)
            {
                _timeLoop = new EventLoop();
                _timeBeat.Add(_timeLoop.Update);
            }

            return _timeLoop.Add(intervalMs, action);
        }
        
        public Looper LoopMs<T>(int intervalMs, Action<T> action, T arg)
        {
            if (null == _timeLoop)
            {
                _timeLoop = new EventLoop();
                _timeBeat.Add(_timeLoop.Update);
            }

            return _timeLoop.Add(intervalMs, action, arg);
        }
        
        public Looper LoopMs<T1, T2>(int intervalMs, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (null == _timeLoop)
            {
                _timeLoop = new EventLoop();
                _timeBeat.Add(_timeLoop.Update);
            }

            return _timeLoop.Add(intervalMs, action, arg1, arg2);
        }

        public Looper LoopMs<T1, T2, T3>(int intervalMs, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (null == _timeLoop)
            {
                _timeLoop = new EventLoop();
                _timeBeat.Add(_timeLoop.Update);
            }

            return _timeLoop.Add(intervalMs, action, arg1, arg2, arg3);
        }
        
        public Looper LoopFrame(int intervalFrame, Action action)
        {
            if (null == _frameLoop)
            {
                _frameLoop = new EventLoop();
                _frameBeat.Add(_frameLoop.Update);
            }

            return _frameLoop.Add(intervalFrame, action);
        }

        public Looper LoopFrame<T>(int intervalFrame, Action<T> action, T arg)
        {
            if (null == _frameLoop)
            {
                _frameLoop = new EventLoop();
                _frameBeat.Add(_frameLoop.Update);
            }

            return _frameLoop.Add(intervalFrame, action, arg);
        }

        public Looper LoopFrame<T1, T2>(int intervalFrame, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (null == _frameLoop)
            {
                _frameLoop = new EventLoop();
                _frameBeat.Add(_frameLoop.Update);
            }

            return _frameLoop.Add(intervalFrame, action, arg1, arg2);
        }

        public Looper LoopFrame<T1, T2, T3>(int intervalFrame, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (null == _frameLoop)
            {
                _frameLoop = new EventLoop();
                _frameBeat.Add(_frameLoop.Update);
            }

            return _frameLoop.Add(intervalFrame, action, arg1, arg2, arg3);
        }
        #endregion

        #region Tween
        public Tweener TweenMs(int durationMs, Action<int> onUpdate, Action<bool> onComplete)
        {
            if (null == _timeTween)
            {
                _timeTween = new EventTween();
                _timeBeat.Add(_timeTween.Update);
            }

            return _timeTween.Add(durationMs, onUpdate, onComplete);
        }

        public Tweener TweenMs<T>(int durationMs, Action<T, int> onUpdate, Action<T, bool> onComplete, T arg)
        {
            if (null == _timeTween)
            {
                _timeTween = new EventTween();
                _timeBeat.Add(_timeTween.Update);
            }

            return _timeTween.Add(durationMs, onUpdate, onComplete, arg);
        }

        public Tweener TweenMs<T1, T2>(int durationMs, Action<T1, T2, int> onUpdate, Action<T1, T2, bool> onComplete, T1 arg1, T2 arg2)
        {
            if (null == _timeTween)
            {
                _timeTween = new EventTween();
                _timeBeat.Add(_timeTween.Update);
            }

            return _timeTween.Add(durationMs, onUpdate, onComplete, arg1, arg2);
        }

        public Tweener TweenMs<T1, T2, T3>(int durationMs, Action<T1, T2, T3, int> onUpdate, Action<T1, T2, T3, bool> onComplete, T1 arg1, T2 arg2, T3 arg3)
        {
            if (null == _timeTween)
            {
                _timeTween = new EventTween();
                _timeBeat.Add(_timeTween.Update);
            }

            return _timeTween.Add(durationMs, onUpdate, onComplete, arg1, arg2, arg3);
        }
        
        public Tweener TweenFrame(int durationMs, Action<int> onUpdate, Action<bool> onComplete)
        {
            if (null == _frameTween)
            {
                _frameTween = new EventTween();
                _frameBeat.Add(_frameTween.Update);
            }

            return _frameTween.Add(durationMs, onUpdate, onComplete);
        }

        public Tweener TweenFrame<T>(int durationMs, Action<T, int> onUpdate, Action<T, bool> onComplete, T arg)
        {
            if (null == _frameTween)
            {
                _frameTween = new EventTween();
                _frameBeat.Add(_frameTween.Update);
            }

            return _frameTween.Add(durationMs, onUpdate, onComplete, arg);
        }

        public Tweener TweenFrame<T1, T2>(int durationMs, Action<T1, T2, int> onUpdate, Action<T1, T2, bool> onComplete, T1 arg1, T2 arg2)
        {
            if (null == _frameTween)
            {
                _frameTween = new EventTween();
                _frameBeat.Add(_frameTween.Update);
            }

            return _frameTween.Add(durationMs, onUpdate, onComplete, arg1, arg2);
        }

        public Tweener TweenFrame<T1, T2, T3>(int durationMs, Action<T1, T2, T3, int> onUpdate, Action<T1, T2, T3, bool> onComplete, T1 arg1, T2 arg2, T3 arg3)
        {
            if (null == _frameTween)
            {
                _frameTween = new EventTween();
                _frameBeat.Add(_frameTween.Update);
            }

            return _frameTween.Add(durationMs, onUpdate, onComplete, arg1, arg2, arg3);
        }

        #endregion
        
        public void CancelAll()
        {
            if (null != _timeDelay) _timeDelay.CancelAll();

            if (null != _frameDelay) _frameDelay.CancelAll();

            if (null != _timeLoop) _timeLoop.CancelAll();

            if (null != _frameLoop) _frameLoop.CancelAll();

            if (null != _timeTween) _timeTween.CancelAll();

            if (null != _frameTween) _frameTween.CancelAll();
        }

        public void PauseAll()
        {
            if (null != _timeDelay) _timeDelay.PauseAll();

            if (null != _frameDelay) _frameDelay.PauseAll();

            if (null != _timeLoop) _timeLoop.PauseAll();

            if (null != _frameLoop) _frameLoop.PauseAll();

            if (null != _timeTween) _timeTween.PauseAll();

            if (null != _frameTween) _frameTween.PauseAll();
        }

        public void ResumeAll()
        {
            if (null != _timeDelay) _timeDelay.ResumeAll();

            if (null != _frameDelay) _frameDelay.ResumeAll();

            if (null != _timeLoop) _timeLoop.ResumeAll();

            if (null != _frameLoop) _frameLoop.ResumeAll();

            if (null != _timeTween) _timeTween.ResumeAll();

            if (null != _frameTween) _frameTween.ResumeAll();
        }
    }
}