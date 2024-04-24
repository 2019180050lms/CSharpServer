using System;
using ServerCore;

namespace Server
{
	class GameRoom : IJobQueue
	{
		List<ClientSession> mSessions = new List<ClientSession>();
		// object mLock = new object();
		JobQueue mJobQueue = new JobQueue();

		// 패킷 모아 보내기
		List<ArraySegment<byte>> mPendingList = new List<ArraySegment<byte>>();

		public void Push(Action job)
		{
			mJobQueue.Push(job);
		}

		public void Flush()
		{
            foreach(ClientSession s in mSessions)
            {
            	s.Send(mPendingList);
            }

			Console.WriteLine($"Flushed {mPendingList.Count} items");

			// JobQueue안에서 하나의 쓰레드만 작업을 실행하니 lock 필요 없음
			mPendingList.Clear();
        }

        public void Enter(ClientSession session)
		{
			//lock (mLock)
			//{
			mSessions.Add(session);
			session.Room = this;
			//}

		}

		public void Leave(ClientSession session)
		{
			//lock (mLock)
			//{
			mSessions.Remove(session);
			//}
		}

        // Command 패턴, 주문서를 만들어서 주문서만 업데이트, 주문서를 본 쓰레드에서 주문서 내용 실행
        // 여러 쓰레드 들은 주문서만 업데이트 하고 빠져 나온다. (기존처럼 주문서 내용을 실행하지는 않음)
        // lock이 아니라 한개의 쓰레드에서 관리하도록 해야함 (lock에서 걸림)
        public void Broadcast(ClientSession session, string chat)
		{
			SC_Chat p = new SC_Chat();
			p.playerId = session.SessionId;
			p.chat = $"{chat}" + $" I am {p.playerId}";
			ArraySegment<byte> segment = p.Write();

            // 공유 변수 접근 (lock)
            //lock (mLock)
            //{

            // N^2의 시간 복잡도 : 자기 자신한테 보내기 + 모든 유저한테 보내기
            // -> 패킷 모아서 보내기 / N^2 -> N
            //foreach(ClientSession s in mSessions)
            //{
            //	s.Send(segment);
            //}
            //}

            mPendingList.Add(segment);

		}

	}
}

