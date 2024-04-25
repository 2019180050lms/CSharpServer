using System;
using ServerCore;

public class PacketManager
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
        mMakeFunc.Add((ushort)PacketID.SC_BroadcastEnterGame, MakePacket<SC_BroadcastEnterGame>);
        mHandler.Add((ushort)PacketID.SC_BroadcastEnterGame, PacketHandler.SC_BroadcastEnterGameHandler);
        mMakeFunc.Add((ushort)PacketID.SC_BroadcastLeaveGame, MakePacket<SC_BroadcastLeaveGame>);
        mHandler.Add((ushort)PacketID.SC_BroadcastLeaveGame, PacketHandler.SC_BroadcastLeaveGameHandler);
        mMakeFunc.Add((ushort)PacketID.SC_PlayerList, MakePacket<SC_PlayerList>);
        mHandler.Add((ushort)PacketID.SC_PlayerList, PacketHandler.SC_PlayerListHandler);
        mMakeFunc.Add((ushort)PacketID.SC_BroadcastMove, MakePacket<SC_BroadcastMove>);
        mHandler.Add((ushort)PacketID.SC_BroadcastMove, PacketHandler.SC_BroadcastMoveHandler);

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