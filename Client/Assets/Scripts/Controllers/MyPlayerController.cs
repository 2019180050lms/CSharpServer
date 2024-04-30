using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
    // MoveDir.None 대신 추가
    bool mMoveKeyPressed = false;

    protected override void Init()
    {
        base.Init();
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

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (mMoveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        // 스킬 상태로 갈지 확인
        if (Input.GetKey(KeyCode.Space) && mCoSkillCooltime == null)
        {
            // TODO: 스페이스바 꾹 눌렀을시 패킷을 계속 보내는 점 쿨타임 처리 추가
            Debug.Log("Skill");

            CS_Skill skill = new CS_Skill() { Info = new SkillInfo() };
            skill.Info.SkillId = 2;
            Managers.Network.Send(skill);

            mCoSkillCooltime = StartCoroutine("CoInputCooltime", 0.2f);
        }
    }

    Coroutine mCoSkillCooltime;
    IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        mCoSkillCooltime = null;
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 3);
    }

    void GetDirInput()
    {
        mMoveKeyPressed = true;

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
            mMoveKeyPressed = false;
        }
    }

    protected override void MoveToNextPos()
    {
        if (mMoveKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }

        Vector3Int destPos = CellPos;
        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.FindCreature(destPos) == null)
            {
                CellPos = destPos;
            }
        }

        CheckUpdatedFlag();
    }

    protected override void CheckUpdatedFlag()
    {
        if (mUpdated)
        {
            CS_Move movePacket = new CS_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            mUpdated = false;
        }
    }
}
