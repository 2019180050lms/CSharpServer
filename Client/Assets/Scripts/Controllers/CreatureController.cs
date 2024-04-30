using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    HpBar mHpBar;

    public override StatInfo Stat
    {
        get { return base.Stat; }
        set
        {
            base.Stat = value;
            UpdateHpBar();
        }
    }

    public override int Hp
    {
        get { return Stat.Hp; }
        set
        {
            base.Stat.Hp = value;
            UpdateHpBar();
        }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 1f, 0);
        go.name = "HpBar";
        mHpBar = go.GetComponent<HpBar>();

        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        if (mHpBar == null)
            return;

        float ratio = 0.0f;
        if(Stat.MaxHp > 0)
            ratio = ((float)Hp) / Stat.MaxHp;

        mHpBar.SetHpBar(ratio);
    }

    protected override void Init()
    {
        base.Init();
        AddHpBar();
    }

    public virtual void OnDamaged()
    {

    }

    public virtual void OnDead()
    {
        State = CreatureState.Dead;

        // TODO: Effect
    }
}
