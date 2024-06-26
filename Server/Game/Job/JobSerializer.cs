﻿using System;
namespace Server.Game
{
	public class JobSerializer
	{
        JobTimer mTimer = new JobTimer();
        Queue<IJob> mJobQueue = new Queue<IJob>();
        object mLock = new object();
        bool mFlush = false; // jobQueue 내용을 실행 할지 안할지 정하는 변수

        // Helper Class
        public void PushAfter(int tickAfter, Action action) { PushAfter(tickAfter, new Job(action)); }
        public void PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1) { PushAfter(tickAfter, new Job<T1>(action, t1)); }
        public void PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2) { PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2)); }
        public void PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3)); }

        public void PushAfter(int tickAfter, IJob job)
        {
            mTimer.Push(job, tickAfter);
        }

        // Helper Class
        public void Push(Action action) { Push(new Job(action)); }
        public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }

        public void Push(IJob job)
        {
            lock (mLock)
            {
                mJobQueue.Enqueue(job);
            }
        }

        public void Flush()
        {
            mTimer.Flush();

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

