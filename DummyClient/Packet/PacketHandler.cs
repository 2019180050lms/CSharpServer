using System;
using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void SC_ChatHandler(PacketSession session, IPacket packet)
    {
        SC_Chat chatPacket = packet as SC_Chat;
        ServerSession serverSession = session as ServerSession;

        //if(chatPacket.playerId == 1)
        {
        //    Console.WriteLine(chatPacket.chat);
        }
    }
}