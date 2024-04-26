using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine mCoSkill;
    bool mRangeSkill = false;

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
        if (mState == CreatureState.Idle)
        {
            mAnimator.Play("IDLE");
        }
        else if (mState == CreatureState.Moving)
        {
            switch (mDir)
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
        else if (mState == CreatureState.Skill)
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
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 3);
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if(Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        // 스킬 상태로 갈지 확인
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            //mCoSkill = StartCoroutine("CoStartPunch");
            mCoSkill = StartCoroutine("CoStartShootBullet");
        }
    }

    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += Vector3.forward * Time.deltaTime * mSpeed;
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += Vector3.back * Time.deltaTime * mSpeed;
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += Vector3.left * Time.deltaTime * mSpeed;
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += Vector3.right * Time.deltaTime * mSpeed;
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
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
