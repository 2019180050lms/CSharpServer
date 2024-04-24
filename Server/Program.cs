using System.Net;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener mListener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            // ex) www.naver.com -> 127.0.0.1
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // (ip주소, 포트번호) 만들기

            // 문지기 생성
            mListener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {

            }
        }
    }
}