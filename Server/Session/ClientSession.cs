using System;
using ServerCore;
using System.Net;
using System.Text;
using Google.Protobuf.Protocol;
using static Google.Protobuf.Protocol.Person.Types;
using Google.Protobuf;

namespace Server
{
    // Client 통신
    class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            /*
            Packet packet = new Packet() { size = 100, packetId = 10 };

            // Send
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer1 = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer1.Length + buffer2.Length);

            Send(sendBuff);
            */

            //Program.Room.Push(() => Program.Room.Enter(this));
            // Program.Room.Enter(this);

            Person person = new Person()
            {
                Name = "Test",
                Id = 123,
                Email = "test@naver.com",
                Phones = { new PhoneNumber { Number = "555-4321", Type = PhoneType.Home } },
            };

            int size = person.CalculateSize();
            byte[] sendBuffer = person.ToByteArray();
            Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
            ushort protocolId = 1;
            Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(person.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            // PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override int OnSend(int numOfBytes)
        {
            // Console.WriteLine($"Transferred bytes: {numOfBytes}");
            return numOfBytes;
        }
    }
}

