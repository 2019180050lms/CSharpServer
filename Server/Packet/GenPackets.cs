using System;
using ServerCore;
using System.Net;
using System.Text;

public enum PacketID
{
    SC_BroadcastEnterGame = 1,
    CS_LeaveGame = 2,
    SC_BroadcastLeaveGame = 3,
    SC_PlayerList = 4,
    CS_Move = 5,
    SC_BroadcastMove = 6,

}


public interface IPacket
{
    ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}


public class SC_BroadcastEnterGame : IPacket
{
    public int playerId;
    public float posX;
    public float posY;
    public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.SC_BroadcastEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.SC_BroadcastEnterGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class CS_LeaveGame : IPacket
{


    public ushort Protocol { get { return (ushort)PacketID.CS_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.CS_LeaveGame);
        count += sizeof(ushort);



        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class SC_BroadcastLeaveGame : IPacket
{
    public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.SC_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.SC_BroadcastLeaveGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(int);

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class SC_PlayerList : IPacket
{

    public class Player
    {
        public bool IsSelf;
        public int playerId;
        public float posX;
        public float posY;
        public float posZ;

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            this.IsSelf = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
            count += sizeof(bool);
            this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
            this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
            this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
            this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
        }

        public bool Write(Span<byte> s, ref ushort count)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.IsSelf);
            count += sizeof(bool);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
            count += sizeof(float);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
            count += sizeof(float);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
            count += sizeof(float);
            return success;
        }
    }

    public List<Player> players = new List<Player>();

    public ushort Protocol { get { return (ushort)PacketID.SC_PlayerList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.players.Clear();
        ushort playerLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        for (int i = 0; i < playerLen; ++i)
        {
            Player player = new Player();
            player.Read(s, ref count);
            players.Add(player);
        }
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.SC_PlayerList);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.players.Count);
        count += sizeof(ushort);
        foreach (Player player in this.players)
        {
            success &= player.Write(s, ref count);
        }

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class CS_Move : IPacket
{
    public float posX;
    public float posY;
    public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.CS_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.CS_Move);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

public class SC_BroadcastMove : IPacket
{
    public int playerId;
    public float posX;
    public float posY;
    public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.SC_BroadcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
        this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.SC_BroadcastMove);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
        count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

