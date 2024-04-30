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
        base.Init();
        AddHpBar();
    }

    protected override void UpdateAnimation()
    {
        if (mAnimator == null)
            return;

        if (State == CreatureState.Idle)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    mAnimator.Play("IDLE");
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case MoveDir.Left:
                    mAnimator.Play("IDLE");
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case MoveDir.Right:
                    mAnimator.Play("IDLE");
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case MoveDir.Down:
                    mAnimator.Play("IDLE");
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    mAnimator.Play(mRangeSkill ? "WALK" : "WALK");
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case MoveDir.Left:
                    mAnimator.Play("WALK");
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case MoveDir.Right:
                    mAnimator.Play("WALK");
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case MoveDir.Down:
                    mAnimator.Play("WALK");
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            // TODO
            switch (Dir)
            {
                case MoveDir.Up:
                    mAnimator.Play(mRangeSkill ? "SHOOT" : "KICK");
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case MoveDir.Left:
                    mAnimator.Play(mRangeSkill ? "SHOOT" : "KICK");
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case MoveDir.Right:
                    mAnimator.Play(mRangeSkill ? "SHOOT" : "KICK");
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case MoveDir.Down:
                    mAnimator.Play(mRangeSkill ? "SHOOT" : "KICK");
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
        }
        else
        {

        }
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    public void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            mCoSkill = StartCoroutine("CoStartPunch");
        }
        else if(skillId == 2)
        {
            mCoSkill = StartCoroutine("CoStartShootBullet");
        }
    }

    protected virtual void CheckUpdatedFlag()
    {

    }

    IEnumerator CoStartPunch()
    {
        // 쿨타임 처리는 서버, 클라 둘다 해야한다
        // -> 혹시라도 공격을 연타하게 되면 패킷을 계속 보내게 되니
        // -> 네트워크 낭비 패킷을 연속해서 날릴 수 없도록 막아야 한다.

        // 대기 시간
        mRangeSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.5f);

        // Case by Case.. 서버 쪽에서 상태를 바꿔줘도 되고 클라에서 바꿔줘도 된다.
        State = CreatureState.Idle;
        mCoSkill = null;
        CheckUpdatedFlag();
    }

    IEnumerator CoStartShootBullet()
    {
        // 대기 시간
        mRangeSkill = true;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        mCoSkill = null;
        CheckUpdatedFlag();
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hit !");
    }
}
