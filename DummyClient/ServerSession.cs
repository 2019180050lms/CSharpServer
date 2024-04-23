using System;
using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    // Server 통신
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> buffer);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            // 유니코드 최대 3byte
            // ex) 'A' 문자는 UNICODE -> 0x000041
            //     '!' 문자는 UNICODE -> 0x000021
            //     'ㅎ' 문자는 UNICODE -> 0x001112

            // 컴퓨터한테 어떻게 알려줄? -> ENCODING

            // UTF-8(외국 문자 우선시) vs UTF-16(c# 캐릭터 타입, c++은 기본 1바이트)
            // UTF-16을 쓰면 영어 한국어 중걱어 전부 2바이트 사용
            // UTF-8,, UTF-16를 서버와 클라이언트를 맞춰줘야 통신 문제 없이 가능
            // ASCII 1바이트
            // 2048 ~ 65535 3바이트 (한글 같은 경우)


            //ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            //ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count));
            //BitConverter.ToInt64(buffer.Array, buffer.Offset + count);

            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            // 패킷 직렬화
            //success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count), packet.size);
            // Span -> 범위를 지정해서 찝어주는 역활
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset + count, openSegment.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset + count, openSegment.Count - count), this.playerId);
            count += 8;

            success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count), (ushort)4);

            if (success == false)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 };

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

