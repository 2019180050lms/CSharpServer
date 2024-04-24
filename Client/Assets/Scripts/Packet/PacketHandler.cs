using System;
using DummyClient;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void SC_ChatHandler(PacketSession session, IPacket packet)
    {
        SC_Chat chatPacket = packet as SC_Chat;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1)
        {
            Debug.Log(chatPacket.chat);

            GameObject go = GameObject.Find("Player");
            if (go == null)
                Debug.Log("Player not found");
            else
                Debug.Log("Player found");
        }
    }
}