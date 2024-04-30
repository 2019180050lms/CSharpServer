using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MonsterController : CreatureController
{
    Coroutine mCoPatrol;
    Coroutine mCoSearch;

    Coroutine mCoSkill;

    [SerializeField]
    Vector3Int mDestCellPos;

    [SerializeField]
    GameObject mTarget;

    [SerializeField]
    float mSearchRange = 5.0f;

    [SerializeField]
    float mSkillRange = 1.0f;

    [SerializeField]
    bool mRangedSkill = false;

    public override CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            base.State = value;

            if (mCoPatrol != null)
            {
                StopCoroutine(mCoPatrol);
                mCoPatrol = null;
            }

            if (mCoSearch != null)
            {
                StopCoroutine(mCoSearch);
                mCoSearch = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;

        mRangedSkill = (Random.Range(0, 2) == 0 ? true : false);

        if (mRangedSkill)
            mSkillRange = 3.0f;
        else
            mSkillRange = 1.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (mCoPatrol == null)
        {
            mCoPatrol = StartCoroutine("CoPatrol");
        }

        if (mCoSearch == null)
        {
            mCoSearch = StartCoroutine("CoSearch");
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = mDestCellPos;
        if(mTarget != null)
        {
            destPos = mTarget.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos;
            if(dir.magnitude <= mSkillRange && (dir.x == 0) || (dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                if (mRangedSkill)
                    mCoSkill = StartCoroutine("CoStartShootBullet");
                else
                    mCoSkill = StartCoroutine("CoStartPunch");
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        // 길을 못 찾거나, 너무 멀으면 포기
        if(path.Count < 2 || (mTarget != null && path.Count > 20))
        {
            mTarget = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);

        if (Managers.Map.CanGo(nextPos) && Managers.Object.FindCreature(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        mAnimator.Play("DEAD");

        Managers.Object.Remove(Id);
        Managers.Resource.Destroy(gameObject, 2f);
    }

    IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for(int i=0; i<10; ++i)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);

            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if(Managers.Map.CanGo(randPos) && Managers.Object.FindCreature(randPos) == null)
            {
                mDestCellPos = randPos;
                State = CreatureState.Moving;

                // 코루틴 끝내기
                yield break;
            }
        }

        State = CreatureState.Idle;
    }

    IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (mTarget != null)
                continue;

            mTarget = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > mSearchRange)
                    return false;

                return true;
            });
        }
    }

    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        mCoSkill = null;
    }

    IEnumerator CoStartShootBullet()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Bullet");
        BulletController bc = go.GetComponent<BulletController>();
        bc.Dir = Dir;
        bc.CellPos = CellPos;

        // 대기 시간
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        mCoSkill = null;
    }
}
