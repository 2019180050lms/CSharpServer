using System;
using ServerCore;
using System.Net;
using System.Text;

namespace Server
{
    // Client 통신
    class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

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

            Program.Room.Push(() => Program.Room.Enter(this));
            // Program.Room.Enter(this);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                // Room.Leave(this);
                Room = null;
            }

            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override int OnSend(int numOfBytes)
        {
            // Console.WriteLine($"Transferred bytes: {numOfBytes}");
            return numOfBytes;
        }
    }
}

