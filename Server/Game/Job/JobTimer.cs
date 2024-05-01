using ServerCore;

namespace Server.Game
{
    struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int execTick; // 실행 시간
        public IJob job;

        public int CompareTo(JobTimerElement other)
        {
            return other.execTick - execTick;
        }
    }

    public class JobTimer
	{
        PriorityQueue<JobTimerElement> mPQ = new PriorityQueue<JobTimerElement>();
        object mLock = new object();

        public void Push(IJob action, int tickAffter = 0)
        {
            JobTimerElement jobElement;
            jobElement.execTick = System.Environment.TickCount + tickAffter;
            jobElement.job = action;

            lock (mLock)
            {
                mPQ.Push(jobElement);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerElement jobElement;

                lock (mLock)
                {
                    if (mPQ.Count == 0)
                        break;

                    jobElement = mPQ.Peek();
                    if (jobElement.execTick > now)
                        break;

                    mPQ.Pop();
                }

                jobElement.job.Execute();
            }
        }
	}
}