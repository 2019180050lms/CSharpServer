using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoginScene : UI_Scene
{
	enum GameObjects
	{
		AccountId
		//PasswordText,
	}

	enum Buttons
	{
		CreateBtr,
		LoginBtr
	}

	public override void Init()
	{
		base.Init();

		Bind<GameObject>(typeof(GameObjects));
		Bind<Button>(typeof(Buttons));

		GetButton((int)Buttons.CreateBtr).gameObject.BindEvent(OnClickCreateButton);
        GetButton((int)Buttons.LoginBtr).gameObject.BindEvent(OnClickLoginButton);
    }

	public void OnClickCreateButton(PointerEventData evt)
	{
		string account = Get<GameObject>((int)GameObjects.AccountId).GetComponent<InputField>().text;
        //string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        CS_CreatePlayer createPacket = new CS_CreatePlayer();
        createPacket.Name = account;
        Managers.Network.Send(createPacket);
        Get<GameObject>((int)GameObjects.AccountId).GetComponent<InputField>().text = "";

    }

    public void OnClickLoginButton(PointerEventData evt)
    {
        string account = Get<GameObject>((int)GameObjects.AccountId).GetComponent<InputField>().text;
        CS_Login loginPacket = new CS_Login();
        loginPacket.UniqueId = account;
        Managers.Network.Send(loginPacket);
        Get<GameObject>((int)GameObjects.AccountId).GetComponent<InputField>().text = "";
    }
}
