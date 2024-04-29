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

        Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();

        Map mMap = new Map();

        public void Init(int mapId)
        {
            mMap.LoadMap(mapId, "../../../../Common/MapData");
        }

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (mLock)
            {
                mPlayers.Add(newPlayer.Info.PlayerId, newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                SC_EnterGame enterPacket = new SC_EnterGame();
                enterPacket.Player = newPlayer.Info;
                newPlayer.Session.Send(enterPacket);

                // 본인한테 기존 유저 정보 전송
                SC_Spawn spawnPacket = new SC_Spawn();
                foreach (Player p in mPlayers.Values)
                {
                    if (newPlayer != p)
                        spawnPacket.Players.Add(p.Info);
                }
                newPlayer.Session.Send(spawnPacket);

                // 타인한테 정보 전송
                SC_Spawn spawnNewPacket = new SC_Spawn();
                spawnNewPacket.Players.Add(newPlayer.Info);
                foreach(Player p in mPlayers.Values)
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
                Player player = null;
                if(mPlayers.Remove(playerId, out player) == false)
                    return;

                player.Room = null;

                // 본인한테 정보 전송
                SC_LeaveGame leavePacket = new SC_LeaveGame();
                player.Session.Send(leavePacket);

                // 타인한테 정보 전송
                SC_Despawn despawn = new SC_Despawn();
                despawn.PlayerId.Add(player.Info.PlayerId);
                foreach(Player p in mPlayers.Values)
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
                PositionInfo movePosInfo = move.PosInfo;
                PlayerInfo info = player.Info;

                // 다른 좌표로 이동할 경, 갈 수 있는지 체크
                if(movePosInfo.PosX != info.PosInfo.PosX ||
                    movePosInfo.PosY != info.PosInfo.PosY ||
                    movePosInfo.PosZ != info.PosInfo.PosZ)
                {
                    if (mMap.CanGo(new Vector3Int(movePosInfo.PosX, movePosInfo.PosY, movePosInfo.PosZ)) == false)
                        return;
                }

                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                mMap.ApplyMove(player, new Vector3Int(movePosInfo.PosX, movePosInfo.PosY, movePosInfo.PosZ));

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
                Vector3Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                Player target = mMap.Find(skillPos);
                if(target != null)
                {
                    Console.WriteLine("Hit Player");
                }
            }
        }

        public void Broadcast(IMessage packet)
        {
            lock(mLock)
            {
                foreach(Player p in mPlayers.Values)
                {
                    p.Session.Send(packet);
                }
            }
        }
    }
}
