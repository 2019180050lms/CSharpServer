﻿using Google.Protobuf.Protocol;
using System;
namespace Server.Game
{
	public class GameObject
	{
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            // 32bit - 4byte
            // [(x) ObjectType(7)]  [ id                ]
            // [ . . . . . . . . ]  [........] [........] [........]
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        public GameRoom Room { get; set; }

        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        public float Speed
        {
            get { return Stat.Speed; }
            set { Stat.Speed = value; }
        }

        public int Hp
        {
            get { return Stat.Hp; }
            set { Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp); }
        }

        public MoveDir Dir
        {
            get { return PosInfo.MoveDir; }
            set { PosInfo.MoveDir = value; }
        }

        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }

        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        public virtual void Update()
        {

        }

        public Vector3Int CellPos
        {
            get
            {
                return new Vector3Int(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
            }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
                PosInfo.PosZ = value.z;
            }
        }

        public Vector3Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }

        public static MoveDir GetDirFromVec(Vector3Int dir)
        {
            if (dir.x > 0)
                return MoveDir.Right;
            else if (dir.x < 0)
                return MoveDir.Left;
            else if (dir.y > 0)
                return MoveDir.Up;
            else
                return MoveDir.Down;
        }

        public Vector3Int GetFrontCellPos(MoveDir dir)
        {
            Vector3Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector3Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector3Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector3Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector3Int.right;
                    break;
            }

            return cellPos;
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            Stat.Hp = Math.Max(Stat.Hp - damage, 0);

            SC_ChangeHp changePacket = new SC_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            changePacket.HpDiff = damage;
            Room.Broadcast(changePacket);

            if(Stat.Hp == 0)
            {
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            SC_Die diePacket = new SC_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            GameRoom room = Room;
            room.LeaveGame(Id);

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.MoveDir = MoveDir.Down;
            PosInfo.PosX = 0;
            PosInfo.PosY = 0;
            PosInfo.PosZ = 0;

            room.EnterGame(this);
        }
    }
}

