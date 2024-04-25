using System;
using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void SC_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        SC_BroadcastEnterGame chatPacket = packet as SC_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
    }

    public static void SC_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        SC_BroadcastLeaveGame chatPacket = packet as SC_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;
    }

    public static void SC_PlayerListHandler(PacketSession session, IPacket packet)
    {
        SC_PlayerList chatPacket = packet as SC_PlayerList;
        ServerSession serverSession = session as ServerSession;
    }

    public static void SC_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        SC_BroadcastMove chatPacket = packet as SC_BroadcastMove;
        ServerSession serverSession = session as ServerSession;
    }
}