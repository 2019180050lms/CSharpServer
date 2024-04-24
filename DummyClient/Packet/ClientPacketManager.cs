using System;
using ServerCore;

class PacketManager
{
    #region Singleton
    static PacketManager mInstance;
    public static PacketManager Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new PacketManager();
            return mInstance;
        }
    }
    #endregion

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> mOnRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> mHandler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        mOnRecv.Add((ushort)PacketID.SC_Test, MakePacket<SC_Test>);
        mHandler.Add((ushort)PacketID.SC_Test, PacketHandler.SC_TestHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (mOnRecv.TryGetValue(id, out action))
        {
            action.Invoke(session, buffer);
        }
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (mHandler.TryGetValue(packet.Protocol, out action))
        {
            action.Invoke(session, packet);
        }
    }
}