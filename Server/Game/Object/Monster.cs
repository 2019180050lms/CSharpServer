using Google.Protobuf.Protocol;
using Server.Data;
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

		int mSkillRange = 1;
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
                BroadcastMove();
                return;
			}
			Vector3Int dir = mTarget.CellPos - CellPos;
			int dist = dir.cellDistFromZero;
			if(dist == 0 || dist > mChaseCellDist)
            {
                mTarget = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

			// 오브젝트 무시하고 우선 이동
			List<Vector3Int> path = Room.Map.FindPath(CellPos, mTarget.CellPos, checkObjects: false);

			// 길이 없다
			if(path.Count < 2 || path.Count > mChaseCellDist)
            {
                mTarget = null;
                State = CreatureState.Idle;
				BroadcastMove();
                return;
            }
			// 스킬로 넘어갈지 체크
			if(dist <= mSkillRange && (dir.x == 0 || dir.y == 0))
			{
				mCoolTick = 0;
				State = CreatureState.Skill;
				return;
			}
			// 이동
			Dir = GetDirFromVec(path[1] - CellPos);
			Room.Map.ApplyMove(this, path[1]);
			BroadcastMove();
        }

		void BroadcastMove()
		{
            // 다른 플레이어한테 전송
            SC_Move movePacket = new SC_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }

		long mCoolTick = 0;
        protected virtual void UpdateSkill()
        {
			if(mCoolTick == 0)
			{
				// 유효한 타겟인지 체크
				if (mTarget == null || mTarget.Room != Room || mTarget.Hp == 0)
				{
					mTarget = null;
					State = CreatureState.Moving;
					BroadcastMove();
					return;
				}

				// 스킬이 아직 사용 가능한지
				Vector3Int dir = mTarget.CellPos - CellPos;
				int dist = dir.cellDistFromZero;
				bool canUseSkill = (dist <= mSkillRange && (dir.x == 0 || dir.y == 0));
				if(canUseSkill == false)
				{
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

				// 타겟팅 방향 주시
				MoveDir lookDir = GetDirFromVec(dir);
				if(Dir != lookDir)
				{
					Dir = lookDir;
					BroadcastMove();
				}

				// TODO: 몬스터 데이터 시트 제작
				Skill skillData = null;
				DataManager.SkillDict.TryGetValue(1, out skillData);

				// 데미지 판정
				mTarget.OnDamaged(this, skillData.damage + Stat.Attack);

				// 스킬 사용 Broadcast
				SC_Skill skill = new SC_Skill() { Info = new SkillInfo() };
				skill.ObjectId = Id;
				skill.Info.SkillId = skillData.id;
				Room.Broadcast(skill);

				// 스킬 쿨타임 적용
				int coolTick = (int)(1000 * skillData.cooldown);
				mCoolTick = Environment.TickCount64 + coolTick;
			}

			if (mCoolTick > Environment.TickCount64)
				return;

			mCoolTick = 0;
        }

        protected virtual void UpdateDead()
        {

        }
    }
}

