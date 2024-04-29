using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public int Id { get; set; }

    [SerializeField]
    public float mSpeed = 5.0f;

    protected bool mUpdated = false;

    PositionInfo mPositionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return mPositionInfo; }
        set
        {
            if (mPositionInfo.Equals(value))
                return;

            CellPos = new Vector3Int(value.PosX, value.PosY, value.PosZ);
            State = value.State;
            Dir = value.MoveDir;
        }
    }

    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0, 0.5f);
        transform.position = destPos;
    }

    [SerializeField]
    public Vector3Int CellPos
    {
        
        get { return new Vector3Int(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ); }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y && PosInfo.PosZ == value.z)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            PosInfo.PosZ = value.z;
            mUpdated = true;
        }
    }

    [SerializeField]
    protected Animator mAnimator;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;
            PosInfo.State = value;
            UpdateAnimation();
            mUpdated = true;
        }
    }

    public MoveDir Dir
    {
        get { return PosInfo.MoveDir; }
        set
        {
            if (PosInfo.MoveDir == value)
                return;

            PosInfo.MoveDir = value;

            UpdateAnimation();
            mUpdated = true;
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
        else
            return MoveDir.Down;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
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
        if (mAnimator == null)
            return;

        if(State == CreatureState.Idle)
        {
            mAnimator.Play("IDLE");
        }
        else if(State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    mAnimator.Play("WALK");
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
        else if(State == CreatureState.Skill)
        {
            // TODO
            mAnimator.Play("KICK");
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

        State = CreatureState.Idle;
        Dir = MoveDir.Down;
        UpdateAnimation();
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
