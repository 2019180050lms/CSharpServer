using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    [SerializeField]
    public float mSpeed = 5.0f;

    [SerializeField]
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;

    public Animator mAnimator;

    [SerializeField]
    protected CreatureState mState = CreatureState.Idle;
    public virtual CreatureState State
    {
        get { return mState; }
        set
        {
            if (mState == value)
                return;
            mState = value;
            UpdateAnimation();
        }
    }

    protected MoveDir mLastDir = MoveDir.None;

    [SerializeField]
    protected MoveDir mDir = MoveDir.None;
    public MoveDir Dir
    {
        get { return mDir; }
        set
        {
            if (mDir == value)
                return;

            mDir = value;
            if (mDir != MoveDir.None)
                mLastDir = mDir;

            UpdateAnimation();
        }
    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else if (dir.y < 0)
            return MoveDir.Down;
        else
            return MoveDir.None;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (mLastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
    {
        if(mState == CreatureState.Idle)
        {
            mAnimator.Play("IDLE");
        }
        else if(mState == CreatureState.Moving)
        {
            switch (mDir)
            {
                case MoveDir.Up:
                    mAnimator.Play("RUN_AIM");
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
        else if(mState == CreatureState.Skill)
        {
            // TODO
            mAnimator.Play("SHOOT");
        }
        else
        {

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
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0, 0.5f);
        transform.position = pos;
        mSpeed = 3.0f;
    }

    protected virtual void UpdateController()
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

    // 이동 가능한 상태일 때 실제 좌표 이동
    protected virtual void UpdateIdle()
    {

    }

    // 이동 보정 처리
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < mSpeed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * mSpeed * Time.deltaTime;
            State  = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {
        if(mDir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }

        Vector3Int destPos = CellPos;
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
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged()
    {

    }
}
