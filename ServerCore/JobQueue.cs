
namespace ServerCore
{
	public interface IJobQueue
	{
		void Push(Action job);

	}

	public class JobQueue : IJobQueue
	{
		Queue<Action> mJobQueue = new Queue<Action>();
		object mLock = new object();
		bool mFlush = false; // jobQueue 내용을 실행 할지 안할지 정하는 변수

        public void Push(Action job)
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
				Action action = Pop();
				if (action == null)
					return;

				action.Invoke();
			}
		}

		Action Pop()
		{
			lock(mLock)
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

