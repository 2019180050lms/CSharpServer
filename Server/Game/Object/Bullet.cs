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
			if (Owner == null || Room == null)
				return;

			if (mNextMoveTick >= Environment.TickCount64)
				return;

			mNextMoveTick = Environment.TickCount64 + 50;

			Vector3Int destPos = GetFrontCellPos();
			if (Room.Map.CanGo(destPos))
			{
				CellPos = destPos;

				SC_Move movePacket = new SC_Move();
				movePacket.ObjectId = Id;
				movePacket.PosInfo = PosInfo;
				Room.Broadcast(movePacket);

                Console.WriteLine("Move Bullet");
            }
			else
			{
				GameObject target = Room.Map.Find(destPos);
				if(target != null)
				{
					// TODO: 피격 판정

					// 소멸
					Room.LeaveGame(Id);
				}
			}
		}
	}
}

