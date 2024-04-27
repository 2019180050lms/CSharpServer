using ServerCore;

namespace Server
{
    struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int execTick; // 실행 시간
        public Action action;

        public int CompareTo(JobTimerElement other)
        {
            return other.execTick - execTick;
        }
    }

    class JobTimer
	{
        PriorityQueue<JobTimerElement> mPQ = new PriorityQueue<JobTimerElement>();
        object mLock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAffter = 0)
        {
            JobTimerElement job;
            job.execTick = System.Environment.TickCount + tickAffter;
            job.action = action;

            lock (mLock)
            {
                mPQ.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerElement job;

                lock (mLock)
                {
                    if (mPQ.Count == 0)
                        break;

                    job = mPQ.Peek();
                    if (job.execTick > now)
                        break;

                    mPQ.Pop();
                }

                job.action.Invoke();
            }
        }
	}
}

