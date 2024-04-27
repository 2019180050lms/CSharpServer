using Server;
using ServerCore;

class PacketHandler
{
    public static void CS_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
        // clientSession.Room.Broadcast(clientSession, p.chat);
    }

    public static void CS_MoveHandler(PacketSession session, IPacket packet)
    {
        CS_Move p = packet as CS_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        // Console.WriteLine($"{p.posX}, {p.posY}, {p.posZ}");

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, p));
        // clientSession.Room.Broadcast(clientSession, p.chat);
    }
}