using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
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
        if (Dir != MoveDir.None)
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

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 3);
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
}
