using System;

namespace ServerCore
{
	public class PriorityQueue<T> where T : IComparable<T>
	{
		List<T> mHeap = new List<T>();

        public int Count { get { return mHeap.Count; } }

        // O(logN)
        public void Push(T data)
		{
			// 힙의 맨 끝에 데이터 삽입
			mHeap.Add(data);

			int now = mHeap.Count - 1;

			while(now > 0)
			{
				int next = (now - 1) / 2;
				if (mHeap[now].CompareTo(mHeap[next]) < 0)
					break;

				T temp = mHeap[now];
				mHeap[now] = mHeap[next];
				mHeap[next] = temp;

				now = next;
			}
		}

		public T Pop()
		{
			T ret = mHeap[0];

			int lastIndex = mHeap.Count - 1;
			mHeap[0] = mHeap[lastIndex];
			mHeap.RemoveAt(lastIndex);
			lastIndex--;

			int now = 0;
			while (true)
			{
				int left = 2 * now + 1;
				int right = 2 * now + 2;

				int next = now;
				// 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
				if (left <= lastIndex && mHeap[next].CompareTo(mHeap[left]) < 0)
					next = left;
				// 오른값이 현재값보다 크면, 오른쪽으로 이동
				if (right <= lastIndex && mHeap[next].CompareTo(mHeap[right]) < 0)
					next = right;

				if (next == now)
					break;

				T temp = mHeap[now];
				mHeap[now] = mHeap[next];
				mHeap[next] = temp;

				now = next;
			}

			return ret;
		}

		// 빼지는 않고 뭔가 있는지 확인
		public T Peek()
		{
			if (mHeap.Count == 0)
				return default(T);

			return mHeap[0];
		}
	}
}

