using Google.Protobuf.Protocol;
using System;

namespace Server.Game
{
	public class Monster : GameObject
	{
		public Monster()
		{
			ObjectType = GameObjectType.Monster;

			// 임시
			Stat.Level = 1;
			Stat.Hp = 100;
			Stat.MaxHp = 100;
			Stat.Speed = 5.0f;

			State = CreatureState.Idle;
		}

		// FSM (Finite State Machine)
		public override void Update()
		{
			switch (State)
			{
				case CreatureState.Idle:
					UpdateIdle();
					break;
				case CreatureState.Moving:
					UpdateMoving();
					break;
				case CreatureState.Skill:
					UpdateSkill();
					break;
				case CreatureState.Dead:
					UpdateDead();
					break;
			}
		}

		Player mTarget;
		int mSearchCellDist = 10;
		// 쫓아가는 거리
		int mChaseCellDist = 20;

		long mNextSearchTick = 0;
		protected virtual void UpdateIdle()
		{
			if (mNextSearchTick > Environment.TickCount64)
				return;

			// 1 seconds
			mNextSearchTick = Environment.TickCount64 + 1000;

			Player target = Room.FindPlayer(p =>
			{
				Vector3Int dir = p.CellPos - CellPos;
				return dir.cellDistFromZero <= mSearchCellDist;
			});

			if (target == null)
				return;

			mTarget = target;
			State = CreatureState.Moving;
		}

		long mNextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
			if (mNextMoveTick > Environment.TickCount64)
				return;
			int moveTick = (int)(1000 / Speed);
			mNextMoveTick = Environment.TickCount64 + moveTick;

			// Patrol 모드 일때
			if(mTarget == null || mTarget.Room != Room)
			{
				mTarget = null;
				State = CreatureState.Idle;
				return;
			}

			int dist = (mTarget.CellPos - CellPos).cellDistFromZero;
			if(dist == 0 || dist > mChaseCellDist)
            {
                mTarget = null;
                State = CreatureState.Idle;
                return;
            }

			// 오브젝트 무시하고 우선 이동
			List<Vector3Int> path = Room.Map.FindPath(CellPos, mTarget.CellPos, checkObjects: false);

			// 길이 없다
			if(path.Count < 2 || path.Count > mChaseCellDist)
            {
                mTarget = null;
                State = CreatureState.Idle;
                return;
            }

			// 이동
			Dir = GetDirFromVec(path[1] - CellPos);
			Room.Map.ApplyMove(this, path[1]);

			// 다른 플레이어한테 전송
			SC_Move movePacket = new SC_Move();
			movePacket.ObjectId = Id;
			movePacket.PosInfo = PosInfo;
			Room.Broadcast(movePacket);
        }

        protected virtual void UpdateSkill()
        {

        }

        protected virtual void UpdateDead()
        {

        }
    }
}

