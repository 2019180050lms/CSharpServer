using System;
using ServerCore;

class PacketManager
{
    #region Singleton
    static PacketManager mInstance = new PacketManager();
    public static PacketManager Instance { get { return mInstance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> mMakeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> mHandler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        mMakeFunc.Add((ushort)PacketID.CS_Chat, MakePacket<CS_Chat>);
        mHandler.Add((ushort)PacketID.CS_Chat, PacketHandler.CS_ChatHandler);
        mMakeFunc.Add((ushort)PacketID.CS_PlayerInfoReq, MakePacket<CS_PlayerInfoReq>);
        mHandler.Add((ushort)PacketID.CS_PlayerInfoReq, PacketHandler.CS_PlayerInfoReqHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (mMakeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);

            if (onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);
        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (mHandler.TryGetValue(packet.Protocol, out action))
        {
            action.Invoke(session, packet);
        }
    }
}