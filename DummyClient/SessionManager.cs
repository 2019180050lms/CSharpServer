using System;

namespace DummyClient
{
	class SessionManager
	{
		static SessionManager mSession = new SessionManager();
		public static SessionManager Instance { get { return mSession; } }

		List<ServerSession> mSessions = new List<ServerSession>();
		object mLock = new object();
		Random mRand = new Random();

		public void SendForEach()
		{
			lock (mLock)
			{
				foreach(ServerSession session in mSessions)
				{
					CS_Move move = new CS_Move();
					move.posX =  mRand.Next(-50, 50);
                    move.posY = 0;
                    move.posZ = mRand.Next(-50, 50);
					session.Send(move.Write());
				}
			}
		}

		public ServerSession Generate()
		{
			lock (mLock)
			{
				ServerSession session = new ServerSession();
				mSessions.Add(session);
				return session;
			}
		}
	}
}

