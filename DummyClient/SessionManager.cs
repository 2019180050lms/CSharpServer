using System;

namespace DummyClient
{
	class SessionManager
	{
		static SessionManager mSession = new SessionManager();
		public static SessionManager Instance { get { return mSession; } }

		List<ServerSession> mSessions = new List<ServerSession>();
		object mLock = new object();

		public void SendForEach()
		{
			lock (mLock)
			{
				foreach(ServerSession session in mSessions)
				{
					CS_Chat chatPacket = new CS_Chat();
					chatPacket.chat = $"Test ChatPacket !";
					ArraySegment<byte> segment = chatPacket.Write();

					session.Send(segment);
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

