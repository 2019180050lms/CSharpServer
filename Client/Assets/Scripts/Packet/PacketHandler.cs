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

        Managers.Object.Clear();
    }

    public static void SC_SpawnHandler(PacketSession session, IMessage packet)
    {
        SC_Spawn spawnPacket = packet as SC_Spawn;

        foreach(ObjectInfo obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, myPlayer: false);
        }
    }

    public static void SC_DespawnHandler(PacketSession session, IMessage packet)
    {
        SC_Despawn despawnPacket = packet as SC_Despawn;

        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void SC_MoveHandler(PacketSession session, IMessage packet)
    {
        SC_Move movePacket = packet as SC_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        bc.PosInfo = movePacket.PosInfo;
    }

    public static void SC_SkillHandler(PacketSession session, IMessage packet)
    {
        SC_Skill skillPacket = packet as SC_Skill;

        GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        cc.UseSkill(skillPacket.Info.SkillId);
    }

    public static void SC_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        SC_ChangeHp changePacket = packet as SC_ChangeHp;

        GameObject go = Managers.Object.FindById(changePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        cc.Hp = changePacket.Hp;
    }

    public static void SC_DieHandler(PacketSession session, IMessage packet)
    {
        SC_Die diePacket = packet as SC_Die;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        cc.Hp = 0;
        cc.OnDead();
    }
}
