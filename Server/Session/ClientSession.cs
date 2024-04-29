using System;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using Server.Data;

namespace Server
{
    // Client 통신
    public class ClientSession : PacketSession
    {
        public Player MyPlayer { get; set; }
        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            string msgNameF = msgName[0] + msgName[1].ToString().ToLower() + msgName.Substring(2);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgNameF);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            // 플레이어 생성
            MyPlayer = ObjectManager.Instance.Add<Player>();
            MyPlayer.Info.Name = $"Player_{MyPlayer.Info.ObjectId}";
            MyPlayer.Info.PosInfo.State = CreatureState.Idle;
            MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
            MyPlayer.Info.PosInfo.PosX = 0;
            MyPlayer.Info.PosInfo.PosY = 0;
            MyPlayer.Info.PosInfo.PosZ = 0;
            MyPlayer.Info.PosInfo.RotX = 0;
            MyPlayer.Info.PosInfo.RotY = 40;
            MyPlayer.Info.PosInfo.RotZ = 0;

            StatInfo stat = null;
            DataManager.StatDict.TryGetValue(1, out stat);
            MyPlayer.Stat.MergeFrom(stat);

            MyPlayer.Session = this;

            RoomManager.Instance.Find(1).EnterGame(MyPlayer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            RoomManager.Instance.Find(1).LeaveGame(MyPlayer.Info.ObjectId);

            SessionManager.Instance.Remove(this);

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

