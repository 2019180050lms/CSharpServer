using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    public static void SC_ChatHandler(PacketSession session, IMessage packet)
    {
        SC_Chat chatPacket = packet as SC_Chat;
        ServerSession serverSession = session as ServerSession;

        Debug.Log(chatPacket.Context);
    }

    public static void SC_EnterGameHandler(PacketSession session, IMessage packet)
    {
        SC_EnterGame enterGamePacket = packet as SC_EnterGame;
        ServerSession serverSession = session as ServerSession;
    }
}
