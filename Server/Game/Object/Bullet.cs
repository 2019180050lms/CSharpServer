using Google.Protobuf.Protocol;
using System;
namespace Server.Game
{
	public class Bullet : Projectile
	{
		public GameObject Owner { get; set; }

		long mNextMoveTick = 0;

		public override void Update()
		{
			if (Data == null || Data.projectile == null || Owner == null || Room == null)
				return;

			if (mNextMoveTick >= Environment.TickCount64)
				return;

			long tick = (long)(1000 / Data.projectile.speed);
			mNextMoveTick = Environment.TickCount64 + tick;

			Vector3Int destPos = GetFrontCellPos();
			if (Room.Map.CanGo(destPos))
			{
				CellPos = destPos;

				SC_Move movePacket = new SC_Move();
				movePacket.ObjectId = Id;
				movePacket.PosInfo = PosInfo;
				Room.Broadcast(movePacket);
            }
			else
			{
				GameObject target = Room.Map.Find(destPos);
				if(target != null)
				{
					// TODO: 피격 판정
					target.OnDamaged(this, Data.damage + Owner.Stat.Attack);
				}

				// 소멸
				Room.Push(Room.LeaveGame, Id);
            }
		}
	}
}

