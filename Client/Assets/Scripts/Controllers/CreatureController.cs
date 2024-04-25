using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public float mSpeed = 2.0f;

    protected Vector3Int mCellPos = Vector3Int.zero;
    protected bool mIsMoving = false;
    protected Animator mAnimator;

    MoveDir mDir = MoveDir.None;
    public MoveDir Dir
    {
        get { return mDir; }
        set
        {
            if (mDir == value)
                return;

            switch (value)
            {
                case MoveDir.Down:
                    mAnimator.Play("WALK_BACK");
                    break;
                case MoveDir.Up:
                case MoveDir.Left:
                case MoveDir.Right:
                    mAnimator.Play("WALK");
                    break;
                case MoveDir.None:
                    mAnimator.Play("IDLE");
                    break;
            }
            mDir = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        mAnimator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(mCellPos) + new Vector3(0.5f, 0, 0.5f);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        UpdatePosition();
        UpdateIsMoving();
    }

    // 이동 보정 처리
    void UpdatePosition()
    {
        if (mIsMoving == false)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(mCellPos) + new Vector3(0.5f, 0, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < mSpeed * Time.deltaTime)
        {
            transform.position = destPos;
            mIsMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * mSpeed * Time.deltaTime;
            mIsMoving = true;
        }
    }

    // 이동 가능한 상태일 때 실제 좌표 이동
    void UpdateIsMoving()
    {
        if (mIsMoving == false && mDir != MoveDir.None)
        {
            Vector3Int destPos = mCellPos;
            switch (mDir)
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
                mCellPos = destPos;
                mIsMoving = true;
            }
        }
    }
}
