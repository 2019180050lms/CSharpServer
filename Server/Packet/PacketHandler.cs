using Google.Protobuf;
using Server;
using ServerCore;

class PacketHandler
{
    public static void CS_ChatHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = session as ClientSession;

        //GameRoom room = clientSession.Room;
        //room.Push(() => room.Leave(clientSession));
        // clientSession.Room.Broadcast(clientSession, p.chat);
    }
}