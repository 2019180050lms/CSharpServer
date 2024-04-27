using System.Net;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using static Google.Protobuf.Protocol.Person.Types;

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
            Person person = new Person()
            {
                Name = "Test",
                Id = 123,
                Email = "test@naver.com",
                Phones = { new PhoneNumber { Number = "555-4321", Type = PhoneType.Home } },
            };

            int size = person.CalculateSize();
            byte[] sendBuffer = person.ToByteArray();

            Person person2 = new Person();
            person2.MergeFrom(sendBuffer);

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

            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }
}