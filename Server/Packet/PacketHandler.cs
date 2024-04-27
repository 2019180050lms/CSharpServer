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

        Console.WriteLine($"CS_Move({move.PosInfo.PosX} {move.PosInfo.PosY} {move.PosInfo.PosZ})");

        if (clientSession.MyPlayer == null)
            return;
        if (clientSession.MyPlayer.Room == null)
            return;

        // TODO 검증 (해킹)
        PlayerInfo info = clientSession.MyPlayer.Info;
        info.PosInfo = move.PosInfo;

        // 다른 플레이어한테도 알려준다.
        SC_Move movePacket = new SC_Move();
        movePacket.PlayerId = clientSession.MyPlayer.Info.PlayerId;
        movePacket.PosInfo = movePacket.PosInfo;

        clientSession.MyPlayer.Room.Broadcast(movePacket);
    }
}