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

        public void HandleMove(Player player, CS_Move move)
        {
            if (player == null)
                return;

            // 경합 문제 해결
            lock (mLock)
            {
                // TODO 검증 (해킹)

                PlayerInfo info = player.Info;
                info.PosInfo = move.PosInfo;

                // 다른 플레이어한테도 알려준다.
                SC_Move movePacket = new SC_Move();
                movePacket.PlayerId = player.Info.PlayerId;
                movePacket.PosInfo = move.PosInfo;

                Broadcast(movePacket);
            }
        }

        public void HandleSkill(Player player, CS_Skill skill)
        {
            if (player == null)
                return;

            // Job, Task로 교체
            lock(mLock)
            {
                PlayerInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                // TODO: 스킬 사용 가능 여부 체크

                // 통과
                info.PosInfo.State = CreatureState.Skill;
                SC_Skill ss = new SC_Skill() { Info = new SkillInfo() };
                ss.PlayerId = info.PlayerId;
                ss.Info.SkillId = 1;
                Broadcast(ss);

                // TODO: 데미지 판정
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
