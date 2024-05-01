using System;
namespace Server.Game
{
	public class JobSerializer
	{
        Queue<IJob> mJobQueue = new Queue<IJob>();
        object mLock = new object();
        bool mFlush = false; // jobQueue 내용을 실행 할지 안할지 정하는 변수

        // Helper Class
        public void Push(Action action) { Push(new Job(action)); }
        public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }

        public void Push(IJob job)
        {
            bool flush = false;

            lock (mLock)
            {
                mJobQueue.Enqueue(job);
                if (mFlush == false)
                    flush = mFlush = true;
            }

            if (flush)
                Flush();
        }

        void Flush()
        {
            while (true)
            {
                IJob job = Pop();
                if (job == null)
                    return;

                job.Execute();
            }
        }

        IJob Pop()
        {
            lock (mLock)
            {
                if (mJobQueue.Count == 0)
                {
                    mFlush = false;
                    return null;
                }
                return mJobQueue.Dequeue();
            }
        }
    }
}

