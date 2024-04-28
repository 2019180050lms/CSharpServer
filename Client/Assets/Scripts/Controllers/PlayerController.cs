using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine mCoSkill;
    protected bool mRangeSkill = false;

    protected override void Init()
    {
        switch (mLastDir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }

        base.Init();
    }

    protected override void UpdateAnimation()
    {
        if (mAnimator == null)
            return;

        if (State == CreatureState.Idle)
        {
            mAnimator.Play("IDLE");
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    mAnimator.Play(mRangeSkill ? "RUN_AIM" : "RUN_AIM");
                    transform.rotation = Quaternion.Euler(0, 40, 0);
                    break;
                case MoveDir.Left:
                    mAnimator.Play("RUN_AIM");
                    transform.rotation = Quaternion.Euler(0, -50, 0);
                    break;
                case MoveDir.Right:
                    mAnimator.Play("RUN_AIM");
                    transform.rotation = Quaternion.Euler(0, 130, 0);
                    break;
                case MoveDir.Down:
                    mAnimator.Play("RUN_AIM");
                    transform.rotation = Quaternion.Euler(0, 220, 0);
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            // TODO
            mAnimator.Play("SHOOT");
        }
        else
        {

        }
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if(Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }
    }

    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if(go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        // 대기 시간
        mRangeSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        mCoSkill = null;
    }

    IEnumerator CoStartShootBullet()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Bullet");
        BulletController bc = go.GetComponent<BulletController>();
        bc.Dir = mLastDir;
        bc.CellPos = CellPos;

        // 대기 시간
        mRangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        mCoSkill = null;
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hit !");
    }
}
