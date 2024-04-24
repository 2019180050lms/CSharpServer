using Server;
using ServerCore;

class PacketHandler
{
    public static void CS_ChatHandler(PacketSession session, IPacket packet)
    {
        CS_Chat p = packet as CS_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Broadcast(clientSession, p.chat));
        // clientSession.Room.Broadcast(clientSession, p.chat);
    }

    public static void CS_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        CS_PlayerInfoReq p = packet as CS_PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");

        foreach (CS_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill({skill.id} {skill.level} {skill.duration})");
        }
    }
}