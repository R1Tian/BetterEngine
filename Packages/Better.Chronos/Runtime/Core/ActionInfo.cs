using System;

namespace Better.Chronos
{
    internal abstract class TypeRecycle : IRecycle
    {
        public abstract Type Type { get; }
        public abstract RuntimeTypeHandle TypeHandle { get; }
        
        public bool IsRecycled { get; set; }

        public abstract void Recycle();
    }
    
    internal abstract class BaseActionInfo : TypeRecycle
    {
        public abstract void Execute();
    }

    internal sealed class ActionInfo : BaseActionInfo
    {
        public override Type Type => TypeCache<ActionInfo>.Type;
        public override RuntimeTypeHandle TypeHandle => TypeCache<ActionInfo>.TypeHandle;

        public Action Action;

        public override void Recycle()
        {
            Action = null;
        }

        public override void Execute()
        {
            Action?.Invoke();
        }
    }

    internal sealed class ActionInfo<T> : BaseActionInfo
    {
        public override Type Type => TypeCache<ActionInfo<T>>.Type;
        public override RuntimeTypeHandle TypeHandle => TypeCache<ActionInfo<T>>.TypeHandle;

        public Action<T> Action;
        public T Arg;

        public override void Recycle()
        {
            Action = null;
            Arg = default;
        }

        public override void Execute()
        {
            Action?.Invoke(Arg);
        }
    }

    internal sealed class ActionInfo<T1, T2> : BaseActionInfo
    {
        public override Type Type => TypeCache<ActionInfo<T1, T2>>.Type;
        public override RuntimeTypeHandle TypeHandle => TypeCache<ActionInfo<T1, T2>>.TypeHandle;

        public Action<T1, T2> Action;
        public T1 Arg1;
        public T2 Arg2;

        public override void Recycle()
        {
            Action = null;
            Arg1 = default;
            Arg2 = default;
        }

        public override void Execute()
        {
            Action?.Invoke(Arg1, Arg2);
        }
    }

    internal sealed class ActionInfo<T1, T2, T3> : BaseActionInfo
    {
        public override Type Type => TypeCache<ActionInfo<T1, T2, T3>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(ActionInfo<T1, T2, T3>).TypeHandle;

        public Action<T1, T2, T3> Action;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;

        public override void Recycle()
        {
            Action = null;
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
        }

        public override void Execute()
        {
            Action?.Invoke(Arg1, Arg2, Arg3);
        }
    }

    internal sealed class ActionInfo<T1, T2, T3, T4> : BaseActionInfo
    {
        public override Type Type => TypeCache<ActionInfo<T1, T2, T3, T4>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(ActionInfo<T1, T2, T3, T4>).TypeHandle;

        public Action<T1, T2, T3, T4> Action;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;

        public override void Recycle()
        {
            Action = null;
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
            Arg4 = default;
        }

        public override void Execute()
        {
            Action?.Invoke(Arg1, Arg2, Arg3, Arg4);
        }
    }

    internal abstract class BaseUpdateActionInfo : TypeRecycle
    {
        public abstract void Execute(int thousandth);
    }

    internal sealed class UpdateActionInfo : BaseUpdateActionInfo
    {
        public override Type Type => TypeCache<UpdateActionInfo>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(UpdateActionInfo).TypeHandle;

        public Action<int> Action;

        public override void Recycle()
        {
            Action = null;
        }

        public override void Execute(int thousandth)
        {
            Action?.Invoke(thousandth);
        }
    }

    internal sealed class UpdateActionInfo<T> : BaseUpdateActionInfo
    {
        public override Type Type => TypeCache<UpdateActionInfo<T>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(UpdateActionInfo<T>).TypeHandle;

        public Action<T, int> Action;
        public T Arg;

        public override void Recycle()
        {
            Action = null;
            Arg = default;
        }

        public override void Execute(int thousandth)
        {
            Action?.Invoke(Arg, thousandth);
        }
    }

    internal sealed class UpdateActionInfo<T1, T2> : BaseUpdateActionInfo
    {
        public override Type Type => TypeCache<UpdateActionInfo<T1, T2>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(UpdateActionInfo<T1, T2>).TypeHandle;

        public Action<T1, T2, int> Action;
        public T1 Arg1;
        public T2 Arg2;

        public override void Recycle()
        {
            Action = null;
            Arg1 = default;
            Arg2 = default;
        }

        public override void Execute(int thousandth)
        {
            Action?.Invoke(Arg1, Arg2, thousandth);
        }
    }

    internal sealed class UpdateActionInfo<T1, T2, T3> : BaseUpdateActionInfo
    {
        public override Type Type => TypeCache<UpdateActionInfo<T1, T2, T3>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(UpdateActionInfo<T1, T2, T3>).TypeHandle;

        public Action<T1, T2, T3, int> Action;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;

        public override void Recycle()
        {
            Action = null;
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
        }

        public override void Execute(int thousandth)
        {
            Action?.Invoke(Arg1, Arg2, Arg3, thousandth);
        }
    }

    internal abstract class BaseCompleteActionInfo : TypeRecycle
    {
        public abstract void Execute(bool isBreak);
    }

    internal sealed class CompleteActionInfo : BaseCompleteActionInfo
    {
        public override Type Type => TypeCache<CompleteActionInfo>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(CompleteActionInfo).TypeHandle;

        public Action<bool> Action;

        public override void Recycle()
        {
            Action = null;
        }

        public override void Execute(bool isBreak)
        {
            Action?.Invoke(isBreak);
        }
    }

    internal sealed class CompleteActionInfo<T> : BaseCompleteActionInfo
    {
        public override Type Type => TypeCache<CompleteActionInfo<T>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(CompleteActionInfo<T>).TypeHandle;

        public Action<T, bool> Action;
        public T Arg;

        public override void Recycle()
        {
            Action = null;
            Arg = default;
        }

        public override void Execute(bool isBreak)
        {
            Action?.Invoke(Arg, isBreak);
        }
    }

    internal sealed class CompleteActionInfo<T1, T2> : BaseCompleteActionInfo
    {
        public override Type Type => TypeCache<CompleteActionInfo<T1, T2>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(CompleteActionInfo<T1, T2>).TypeHandle;

        public Action<T1, T2, bool> Action;
        public T1 Arg1;
        public T2 Arg2;

        public override void Recycle()
        {
            Action = null;
            Arg1 = default;
            Arg2 = default;
        }

        public override void Execute(bool isBreak)
        {
            Action?.Invoke(Arg1, Arg2, isBreak);
        }
    }

    internal sealed class CompleteActionInfo<T1, T2, T3> : BaseCompleteActionInfo
    {
        public override Type Type => TypeCache<CompleteActionInfo<T1, T2, T3>>.Type;
        public override RuntimeTypeHandle TypeHandle => typeof(CompleteActionInfo<T1, T2, T3>).TypeHandle;

        public Action<T1, T2, T3, bool> Action;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;

        public override void Recycle()
        {
            Action = null;
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
        }

        public override void Execute(bool isBreak)
        {
            Action?.Invoke(Arg1, Arg2, Arg3, isBreak);
        }
    }
}