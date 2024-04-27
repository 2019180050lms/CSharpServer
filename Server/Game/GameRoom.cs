using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GameRoom
    {
        object mLock = new object();
        public int RoomId { get; set; }

        List<Player> mPlayers = new List<Player>();

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (mLock)
            {
                mPlayers.Add(newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                SC_EnterGame enterPacket = new SC_EnterGame();
                enterPacket.Player = newPlayer.Info;
                newPlayer.Session.Send(enterPacket);

                // 본인한테 기존 유저 정보 전송
                SC_Spawn spawnPacket = new SC_Spawn();
                foreach (Player p in mPlayers)
                {
                    if (newPlayer != p)
                        spawnPacket.Players.Add(p.Info);
                }
                newPlayer.Session.Send(spawnPacket);

                // 타인한테 정보 전송
                SC_Spawn spawnNewPacket = new SC_Spawn();
                spawnNewPacket.Players.Add(newPlayer.Info);
                foreach(Player p in mPlayers)
                {
                    if (newPlayer != p)
                        p.Session.Send(spawnNewPacket);
                }
            }
        }

        public void LeaveGame(int playerId)
        {
            lock (mLock)
            {
                Player player = mPlayers.Find(p => p.Info.PlayerId == playerId);
                if (player == null)
                    return;

                mPlayers.Remove(player);
                player.Room = null;

                // 본인한테 정보 전송
                SC_LeaveGame leavePacket = new SC_LeaveGame();
                player.Session.Send(leavePacket);

                // 타인한테 정보 전송
                SC_Despawn despawn = new SC_Despawn();
                despawn.PlayerId.Add(player.Info.PlayerId);
                foreach(Player p in mPlayers)
                {
                    if (player != p)
                        p.Session.Send(despawn);
                }
            }
        }

        public void Broadcast(IMessage packet)
        {
            lock(mLock)
            {
                foreach(Player p in mPlayers)
                {
                    p.Session.Send(packet);
                }
            }
        }
    }
}
