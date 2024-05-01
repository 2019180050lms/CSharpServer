using System;
namespace Server.Game
{
	public interface IJob
	{
		void Execute();
	}

    public class Job : IJob
    {
        Action mAction;

        public Job(Action action)
        {
            mAction = action;
        }

        public void Execute()
        {
            mAction.Invoke();
        }
    }

    public class Job<T1> : IJob
    {
        Action<T1> mAction;
        T1 mT1;

        public Job(Action<T1> action, T1 t1)
        {
            mAction = action;
            mT1 = t1;
        }

        public void Execute()
        {
            mAction.Invoke(mT1);
        }
    }

    public class Job<T1, T2> : IJob
    {
        Action<T1, T2> mAction;
        T1 mT1;
        T2 mT2;

        public Job(Action<T1, T2> action, T1 t1, T2 t2)
        {
            mAction = action;
            mT1 = t1;
            mT2 = t2;
        }

        public void Execute()
        {
            mAction.Invoke(mT1, mT2);
        }
    }

    public class Job<T1, T2, T3> : IJob
    {
        Action<T1, T2, T3> mAction;
        T1 mT1;
        T2 mT2;
        T3 mT3;

        public Job(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            mAction = action;
            mT1 = t1;
            mT2 = t2;
            mT3 = t3;
        }

        public void Execute()
        {
            mAction.Invoke(mT1, mT2, mT3);
        }
    }
}

