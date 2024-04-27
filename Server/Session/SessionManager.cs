namespace Server
{
	class SessionManager
	{
		static SessionManager mSession = new SessionManager();
		public static SessionManager Instance { get { return mSession; } }

		int mSessionId = 0;
		Dictionary<int, ClientSession> mSessions = new Dictionary<int, ClientSession>();
		object mLock = new object();

		public ClientSession Generate()
		{
			lock (mLock)
			{
				int sessionId = ++mSessionId;

				ClientSession session = new ClientSession();
				session.SessionId = sessionId;
				mSessions.Add(sessionId, session);

				Console.WriteLine($"Connected: {sessionId}");
				return session;
			}
		}

		public ClientSession Find(int id)
		{
			lock (mLock)
			{
				ClientSession session = null;
				mSessions.TryGetValue(id, out session);
				return session;
			}
		}

		public void Remove(ClientSession session)
		{
			lock (mLock)
			{
				mSessions.Remove(session.SessionId);
			}
		}
	}
}

