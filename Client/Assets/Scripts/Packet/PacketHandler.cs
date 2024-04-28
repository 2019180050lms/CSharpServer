using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    public static void SC_EnterGameHandler(PacketSession session, IMessage packet)
    {
        SC_EnterGame enterGamePacket = packet as SC_EnterGame;

        Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
    }

    public static void SC_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        SC_LeaveGame leaveGamePacket = packet as SC_LeaveGame;

        Managers.Object.RemoveMyPlayer();
    }

    public static void SC_SpawnHandler(PacketSession session, IMessage packet)
    {
        SC_Spawn spawnPacket = packet as SC_Spawn;

        foreach(PlayerInfo player in spawnPacket.Players)
        {
            Managers.Object.Add(player, myPlayer: false);
        }
    }

    public static void SC_DespawnHandler(PacketSession session, IMessage packet)
    {
        SC_Despawn despawnPacket = packet as SC_Despawn;

        foreach (int playerId in despawnPacket.PlayerId)
        {
            Managers.Object.Remove(playerId);
        }
    }

    public static void SC_MoveHandler(PacketSession session, IMessage packet)
    {
        SC_Move movePacket = packet as SC_Move;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(movePacket.PlayerId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        cc.PosInfo = movePacket.PosInfo;
    }
}
