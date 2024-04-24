using System;
using ServerCore;
using System.Net;
using System.Text;

public enum PacketID
{
    CS_Chat = 1,
    SC_Chat = 2,
    CS_PlayerInfoReq = 3,

}


interface IPacket
{
    ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}


class CS_Chat : IPacket
{
    public string chat;

    public ushort Protocol { get { return (ushort)PacketID.CS_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
        count += chatLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.CS_Chat);
        count += sizeof(ushort);

        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, openSegment.Array, openSegment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen); // (ushort)993);
        count += sizeof(ushort);
        count += chatLen;

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

class SC_Chat : IPacket
{
    public int playerId;
    public string chat;

    public ushort Protocol { get { return (ushort)PacketID.SC_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
        count += chatLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.SC_Chat);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(int);
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, openSegment.Array, openSegment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen); // (ushort)993);
        count += sizeof(ushort);
        count += chatLen;

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

class CS_PlayerInfoReq : IPacket
{
    public byte testByte;
    public long playerId;
    public string name;

    public class Skill
    {
        public int id;
        public short level;
        public float duration;

        public class Attribute
        {
            public int att;

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
            }

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
                count += sizeof(int);
                return success;
            }
        }

        public List<Attribute> attributes = new List<Attribute>();

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
            this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(short);
            this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
            this.attributes.Clear();
            ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            for (int i = 0; i < attributeLen; ++i)
            {
                Attribute attribute = new Attribute();
                attribute.Read(s, ref count);
                attributes.Add(attribute);
            }
        }

        public bool Write(Span<byte> s, ref ushort count)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
            count += sizeof(short);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
            count += sizeof(float);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.attributes.Count);
            count += sizeof(ushort);
            foreach (Attribute attribute in this.attributes)
            {
                success &= attribute.Write(s, ref count);
            }
            return success;
        }
    }

    public List<Skill> skills = new List<Skill>();

    public ushort Protocol { get { return (ushort)PacketID.CS_PlayerInfoReq; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.testByte = (byte)segment.Array[segment.Offset + count];
        count += sizeof(byte);
        this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
        count += sizeof(long);
        ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
        count += nameLen;
        this.skills.Clear();
        ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        for (int i = 0; i < skillLen; ++i)
        {
            Skill skill = new Skill();
            skill.Read(s, ref count);
            skills.Add(skill);
        }
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

        count += sizeof(ushort); // packet size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.CS_PlayerInfoReq);
        count += sizeof(ushort);

        openSegment.Array[openSegment.Offset + count] = (byte)this.testByte;
        count += sizeof(byte);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(long);
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, openSegment.Array, openSegment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen); // (ushort)993);
        count += sizeof(ushort);
        count += nameLen;
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skills.Count);
        count += sizeof(ushort);
        foreach (Skill skill in this.skills)
        {
            success &= skill.Write(s, ref count);
        }

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

