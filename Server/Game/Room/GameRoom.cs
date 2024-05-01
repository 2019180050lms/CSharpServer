using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }

        Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();
        Dictionary<int, Monster> mMonsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> mProjectiles = new Dictionary<int, Projectile>();

        public Map Map { get; private set; } = new Map();

        public void Init(int mapId)
        {
            Map.LoadMap(mapId, "../../../../Common/MapData");

            // 임시
            Monster monster = ObjectManager.Instance.Add<Monster>();
            monster.CellPos = new Vector3Int(5, 5, 5);
            Push(EnterGame, monster);
        }

        public void Update()
        {
            foreach (Monster monster in mMonsters.Values)
            {
                monster.Update();
            }

            foreach (Projectile projectile in mProjectiles.Values)
            {
                projectile.Update();
            }
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                Player player = gameObject as Player;
                mPlayers.Add(gameObject.Id, player);
                player.Room = this;
                Map.ApplyMove(player, new Vector3Int(player.CellPos.x, player.CellPos.y, player.CellPos.z));

                // 본인한테 정보 전송
                SC_EnterGame enterPacket = new SC_EnterGame();
                enterPacket.Player = player.Info;
                player.Session.Send(enterPacket);

                // 본인한테 기존 유저 정보 전송
                SC_Spawn spawnPacket = new SC_Spawn();
                foreach (Player p in mPlayers.Values)
                {
                    if (player != p)
                        spawnPacket.Objects.Add(p.Info);
                }

                foreach (Monster m in mMonsters.Values)
                {
                    spawnPacket.Objects.Add(m.Info);
                }

                foreach (Projectile p in mProjectiles.Values)
                {
                    spawnPacket.Objects.Add(p.Info);
                }

                player.Session.Send(spawnPacket);
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                mMonsters.Add(gameObject.Id, monster);
                monster.Room = this;

                Map.ApplyMove(monster, new Vector3Int(monster.CellPos.x, monster.CellPos.y, monster.CellPos.z));
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                mProjectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
            }

            // 타인한테 정보 전송
            SC_Spawn spawnNewPacket = new SC_Spawn();
            spawnNewPacket.Objects.Add(gameObject.Info);
            foreach (Player p in mPlayers.Values)
            {
                if (p.Id != gameObject.Id)
                    p.Session.Send(spawnNewPacket);
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (mPlayers.Remove(objectId, out player) == false)
                    return;

                Map.ApplyLeave(player);
                player.Room = null;

                // 본인한테 정보 전송
                SC_LeaveGame leavePacket = new SC_LeaveGame();
                player.Session.Send(leavePacket);
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (mMonsters.Remove(objectId, out monster) == false)
                    return;

                Map.ApplyLeave(monster);
                monster.Room = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (mProjectiles.Remove(objectId, out projectile) == false)
                    return;

                projectile.Room = null;
            }


            // 타인한테 정보 전송
            SC_Despawn despawn = new SC_Despawn();
            despawn.ObjectIds.Add(objectId);
            foreach (Player p in mPlayers.Values)
            {
                if (p.Id != objectId)
                    p.Session.Send(despawn);
            }
        }

        public void HandleMove(Player player, CS_Move move)
        {
            if (player == null)
                return;

            PositionInfo movePosInfo = move.PosInfo;
            ObjectInfo info = player.Info;

            // 다른 좌표로 이동할 경, 갈 수 있는지 체크
            if (movePosInfo.PosX != info.PosInfo.PosX ||
                movePosInfo.PosY != info.PosInfo.PosY ||
                movePosInfo.PosZ != info.PosInfo.PosZ)
            {
                if (Map.CanGo(new Vector3Int(movePosInfo.PosX, movePosInfo.PosY, movePosInfo.PosZ)) == false)
                    return;
            }

            info.PosInfo.State = movePosInfo.State;
            info.PosInfo.MoveDir = movePosInfo.MoveDir;
            Map.ApplyMove(player, new Vector3Int(movePosInfo.PosX, movePosInfo.PosY, movePosInfo.PosZ));

            // 다른 플레이어한테도 알려준다.
            SC_Move movePacket = new SC_Move();
            movePacket.ObjectId = player.Info.ObjectId;
            movePacket.PosInfo = move.PosInfo;

            Broadcast(movePacket);
        }

        public void HandleSkill(Player player, CS_Skill skill)
        {
            if (player == null)
                return;

            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;

            // TODO: 스킬 사용 가능 여부 체크
            info.PosInfo.State = CreatureState.Skill;
            SC_Skill ss = new SC_Skill() { Info = new SkillInfo() };
            ss.ObjectId = info.ObjectId;
            ss.Info.SkillId = skill.Info.SkillId;
            Broadcast(ss);

            Data.Skill skillData = null;
            if (DataManager.SkillDict.TryGetValue(skill.Info.SkillId, out skillData) == false)
                return;

            switch (skillData.skillType)
            {
                case SkillType.SkillAuto:
                    {
                        // TODO: 데미지 판정
                        Vector3Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                        GameObject target = Map.Find(skillPos);
                        if (target != null)
                        {
                            Console.WriteLine("Hit GameObject");
                        }
                    }
                    break;
                case SkillType.SkillProjectile:
                    {
                        // 총알 계산을 서버에서 해야하나?
                        // 총알의 존재를 모두에게 알려야 정확한 동기화가 가능
                        Bullet bullet = ObjectManager.Instance.Add<Bullet>();
                        if (bullet == null)
                            return;

                        bullet.Owner = player;
                        bullet.Data = skillData;
                        bullet.PosInfo.State = CreatureState.Moving;
                        bullet.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        bullet.PosInfo.PosX = player.PosInfo.PosX;
                        bullet.PosInfo.PosY = player.PosInfo.PosY;
                        bullet.PosInfo.PosZ = player.PosInfo.PosZ;
                        bullet.Speed = skillData.projectile.speed;
                        Push(EnterGame, bullet);
                    }
                    break;
            }

            // 통과
            if (skill.Info.SkillId == 1)
            {

            }
            else if (skill.Info.SkillId == 2)
            {

            }
        }

        // TODO: Lock Check
        public Player FindPlayer(Func<GameObject, bool> condition)
        {
            foreach(Player player in mPlayers.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }

        public void Broadcast(IMessage packet)
        {
            foreach (Player p in mPlayers.Values)
            {
                p.Session.Send(packet);
            }
        }
    }
}
