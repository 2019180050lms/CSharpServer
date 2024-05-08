using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScene : BaseScene
{
    UI_LoginScene mSceneUI;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        Screen.SetResolution(1024, 768, false);

        mSceneUI = Managers.UI.ShowSceneUI<UI_LoginScene>();

    }

    public override void Clear()
    {
        
    }
}
