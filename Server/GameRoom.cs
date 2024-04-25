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

			// Console.WriteLine($"Flushed {mPendingList.Count} items");

			// JobQueue안에서 하나의 쓰레드만 작업을 실행하니 lock 필요 없음
			mPendingList.Clear();
        }

        public void Enter(ClientSession session)
        {
            // 플레이어 추가
            //lock (mLock)
            //{
            mSessions.Add(session);
			session.Room = this;
			//}

			// 신입생한테 모든 플레이어 목록 전송
			SC_PlayerList players = new SC_PlayerList();
			foreach(ClientSession s in mSessions)
			{
				players.players.Add(new SC_PlayerList.Player()
				{
					IsSelf = (s == session),
					playerId = s.SessionId,
					posX = s.PosX,
                    posY = s.PosX,
                    posZ = s.PosX,
                });
			}
			session.Send(players.Write());

			// 신입생 입장 모든 플레이어한테 전송
			SC_BroadcastEnterGame enter = new SC_BroadcastEnterGame();
			enter.playerId = session.SessionId;
			enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
			Broadcast(enter.Write());
        }

		public void Leave(ClientSession session)
		{
			// 플레이어 제거
			//lock (mLock)
			//{
			mSessions.Remove(session);
			//}

			// 모두에게 알린다
			SC_BroadcastLeaveGame leave = new SC_BroadcastLeaveGame();
			leave.playerId = session.SessionId;
			Broadcast(leave.Write());

		}

        // Command 패턴, 주문서를 만들어서 주문서만 업데이트, 주문서를 본 쓰레드에서 주문서 내용 실행
        // 여러 쓰레드 들은 주문서만 업데이트 하고 빠져 나온다. (기존처럼 주문서 내용을 실행하지는 않음)
        // lock이 아니라 한개의 쓰레드에서 관리하도록 해야함 (lock에서 걸림)
        public void Broadcast(ArraySegment<byte> segment)
		{
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

		public void Move(ClientSession session, CS_Move packet)
		{
			// 좌표 변경
			session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

			// 모두에게 알린다
			SC_BroadcastMove move = new SC_BroadcastMove();
			move.playerId = session.SessionId;
			move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
			Broadcast(move.Write());
        }
    }
}

