using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    Transform mHpBar = null;


    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0, 1);
        mHpBar.localScale = new Vector3(ratio, 1, 1);
    }
}
