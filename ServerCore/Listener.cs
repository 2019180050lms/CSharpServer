using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
	public class Listener
	{
		Socket mListenSocket;
        Func<Session> mSessionFactory;

		public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
		{
            mListenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mSessionFactory = sessionFactory;

            // 문지기 설정 (ip주소 연결)
            mListenSocket.Bind(endPoint);

            // 영업 시작
            // 10 = 최대 대기수(동접 수) / 숫자 초과시 바로 fail
            mListenSocket.Listen(backlog);

            for (int i = 0; i < register; ++i)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        // NonBlocking
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 기존의 내용 정리
            args.AcceptSocket = null;

            bool pending = mListenSocket.AcceptAsync(args);
            if (pending == false)   // 실행과 동시에 완료
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                // TODO Accept 성공 후 처리
                Session session = mSessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }

        /*
        public Socket Accept()
        {
            // 클라이언트 연결 요청 - Blocking
            return mListenSocket.Accept();
        }
        */
    }
}

