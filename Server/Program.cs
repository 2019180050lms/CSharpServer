using System.Net;
using Server.Data;
using Server.Game;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener mListener = new Listener();

        static void FlushRoom()
        {
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();

            var d = DataManager.StatDict;

            RoomManager.Instance.Add(1);

            // DNS (Domain Name System)
            // ex) www.naver.com -> 127.0.0.1
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            //IPAddress ipAddr = ipHost.AddressList[0];
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // (ip주소, 포트번호) 만들기

            // 문지기 생성
            mListener.Init(endPoint, () => { return SessionManager.Instance.Generate(); }, 10, 200);
            Console.WriteLine("Listening...");

            // FlushRoom();
            JobTimer.Instance.Push(FlushRoom, 0);


            // TODO: JobTimer
            while (true)
            {
                //JobTimer.Instance.Flush();
                RoomManager.Instance.Find(1).Update();

                Thread.Sleep(100);
            }
        }
    }
}