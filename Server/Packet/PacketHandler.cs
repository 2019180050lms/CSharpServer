using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.DB;
using Server.Game;
using ServerCore;
using System.Numerics;

class PacketHandler
{
    public static void CS_MoveHandler(PacketSession session, IMessage packet)
    {
        CS_Move move = packet as CS_Move;
        ClientSession clientSession = session as ClientSession;

        // Console.WriteLine($"CS_Move({move.PosInfo.PosX} {move.PosInfo.PosY} {move.PosInfo.PosZ})");

        // 멀티 쓰레드 환경에서 위험 Player를 꺼내서 체크 해야함
        // 다른 쓰레드에서 MyPlayer를 null로 밀 수 있기 때문, Room도 마찬가지
        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;


        // GameRoom에서 정보가 수정하고 여기서도 수정하게 되면 경합이 발생
        // 정보를 수정할때는 한 쪽에서 정보를 수정하도록 몰아줘야한다.
        room.Push(room.HandleMove, player, move);
    }

    public static void CS_SkillHandler(PacketSession session, IMessage packet)
    {
        CS_Skill skill = packet as CS_Skill;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleSkill, player, skill);
    }

    public static void CS_LoginHandler(PacketSession session, IMessage packet)
    {
        CS_Login loginPacket = packet as CS_Login;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"UniqueId {loginPacket.UniqueId}");

        clientSession.HandleLogin(loginPacket);
    }

    public static void CS_EnterGameHandler(PacketSession session, IMessage packet)
    {
        CS_EnterGame enterGamePacket = (CS_EnterGame)packet;
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandleEnterGame(enterGamePacket);

    }

    public static void CS_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        CS_CreatePlayer createPlayerPacket = (CS_CreatePlayer)packet;
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandleCreatePlayer(createPlayerPacket);

    }
}