using System;
using System.Collections.Generic;

namespace Better
{
    public class HeartBeatActionException : Exception
    {
        public HeartBeatActionException(string message) : base(message)
        {
        }
    }

    public class HeartBeat
    {
        readonly List<Action<int>> _actions = new List<Action<int>>();
        readonly List<Action<int>> _executeActions = new List<Action<int>>();

        public void Add(Action<int> action)
        {
            _actions.Add(action);
        }

        public void Remove(Action<int> action)
        {
            _actions.Remove(action);
        }

        public void Update(int delta)
        {
            _executeActions.Clear();
            foreach (var action in _actions)
            {
                _executeActions.Add(action);
            }

            foreach (var action in _executeActions)
            {
                if (null != action)
                {
                    try
                    {
                        action(delta);
                    }
                    catch (Exception e)
                    {
                        throw new HeartBeatActionException(e.Message + "\n" + e.StackTrace);
                    }
                }
            }
            _executeActions.Clear();
        }
    }
}