using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
    public static void SC_EnterGameHandler(PacketSession session, IMessage packet)
    {
        SC_EnterGame enterGamePacket = packet as SC_EnterGame;

        //anagers.Scene.LoadScene(Define.Scene.Game);
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

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
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

    public static void SC_ConnectedHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("SC_ConnectedHandler");
        // CS_Login loginPacket = new CS_Login();
        // loginPacket.UniqueId = SystemInfo.deviceUniqueIdentifier;
     
        Managers.Scene.LoadScene(Define.Scene.Login);
        // Managers.Network.Send(loginPacket);
    }

    // 로그인 OK + 캐릭터 목록
    public static void SC_LoginHandler(PacketSession session, IMessage packet)
    {
        SC_Login loginPacket = packet as SC_Login;
        Debug.Log($"LoginOk: {loginPacket.LoginOk}");

        // TODO: 로비 목록에서 캐릭터 보여주고 선택할 수 있도록
        if(loginPacket.Players == null || loginPacket.Players.Count == 0)
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
            CS_CreatePlayer createPacket = new CS_CreatePlayer();
            createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
            Managers.Network.Send(createPacket);
        }
        else
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
            // 무조건 첫번째 캐릭으로 로그인
            LobbyPlayerInfo info = loginPacket.Players[0];
            CS_EnterGame enterGamePacket = new CS_EnterGame();
            enterGamePacket.Name = info.Name;
            Managers.Network.Send(enterGamePacket);
        }
    }

    public static void SC_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        SC_CreatePlayer createOkPacket = packet as SC_CreatePlayer;

        if(createOkPacket.Player == null)
        {
            CS_CreatePlayer createPacket = new CS_CreatePlayer();
            createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
            Managers.Network.Send(createPacket);
        }
        else
        {
            CS_EnterGame enterGamePacket = new CS_EnterGame();
            enterGamePacket.Name = createOkPacket.Player.Name;
            Managers.Network.Send(enterGamePacket);
        }
    }
}
