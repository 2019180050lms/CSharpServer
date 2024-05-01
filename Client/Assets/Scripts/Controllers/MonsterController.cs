using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MonsterController : CreatureController
{
    Coroutine mCoSkill;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    public override void OnDamaged()
    {
        //mAnimator.Play("DEAD");

        //Managers.Object.Remove(Id);
        //Managers.Resource.Destroy(gameObject, 2f);
    }

    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            State = CreatureState.Skill;
        }
        else if (skillId == 2)
        {
            // TODO
        }
    }

    /* TODO: 서버로 이전, Patrol
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
    */
}
