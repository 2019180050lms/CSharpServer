using System;
using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    // Server 통신
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            CS_PlayerInfoReq packet = new CS_PlayerInfoReq() { playerId = 1001, name = "TEST" };
            packet.skills.Add(new CS_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f });

            var skill = new CS_PlayerInfoReq.Skill() { id = 1001, level = 12, duration = 7.0f };
            skill.attributes.Add(new CS_PlayerInfoReq.Skill.Attribute() { att = 77 });
            packet.skills.Add(skill);

            packet.skills.Add(new CS_PlayerInfoReq.Skill() { id = 102, level = 2, duration = 4.0f });
            packet.skills.Add(new CS_PlayerInfoReq.Skill() { id = 103, level = 3, duration = 5.0f });
            packet.skills.Add(new CS_PlayerInfoReq.Skill() { id = 104, level = 4, duration = 6.0f });

            // Send
            {
                ArraySegment<byte> s = packet.Write();

                if (s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override int OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
            return numOfBytes;
        }
    }
}

