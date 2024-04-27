using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

class PacketHandler
{
    public static void CS_MoveHandler(PacketSession session, IMessage packet)
    {
        CS_Move move = packet as CS_Move;
        ClientSession clientSession = session as ClientSession;

        //GameRoom room = clientSession.Room;
        //room.Push(() => room.Leave(clientSession));
        // clientSession.Room.Broadcast(clientSession, p.chat);
    }
}