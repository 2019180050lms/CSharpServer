using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginScene : BaseScene
{
    EventSystem system;

    public Text AccountID;
    public Button loginButton;

    protected override void Init()
    {
        base.Init();
        System.Console.WriteLine("Login Scene Test");
        SceneType = Define.Scene.Login;
        system = EventSystem.current;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string Id = AccountID.text;
            loginButton.onClick.Invoke();
            CS_Login loginPacket = new CS_Login();
            loginPacket.UniqueId = Id;
            Managers.Network.Send(loginPacket);
        }
    }

    public override void Clear()
    {

    }
}
