using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            // ex) www.naver.com -> 127.0.0.1
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // (ip주소, 포트번호) 만들기

            // 문지기 생성
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 문지기 설정 (ip주소 연결)
                listenSocket.Bind(endPoint);

                // 영업 시작
                // 10 = 최대 대기수(동접 수) / 숫자 초과시 바로 fail
                listenSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening...");

                    // 클라이언트 연결 요청 - Blocking
                    Socket clientSocket = listenSocket.Accept();

                    // Recv
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = clientSocket.Receive(recvBuff);

                    // 받은 byte를 string으로 변환
                    //                               시작      크기 
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Client] {recvData}");

                    // Send
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Server !");
                    clientSocket.Send(sendBuff);

                    // 연결 종료
                    clientSocket.Shutdown(SocketShutdown.Both); // 연결 종료 전 알림 [옵션]
                    clientSocket.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}