using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> mObjects = new Dictionary<int, GameObject>();


    public void Add(PlayerInfo info, bool myPlayer = false)
    {
        if (myPlayer)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
            go.name = info.Name;
            mObjects.Add(info.PlayerId, go);

            MyPlayer = go.GetComponent<MyPlayerController>();
            MyPlayer.Id = info.PlayerId;
            MyPlayer.CellPos = new Vector3Int(info.PosX, info.PosY, info.PosZ);
            MyPlayer.transform.rotation = Quaternion.Euler(info.RotX, info.RotY, info.RotZ);
        }
        else
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Player");
            go.name = info.Name;
            mObjects.Add(info.PlayerId, go);

            PlayerController pc = go.GetComponent<PlayerController>();
            pc.Id = info.PlayerId;
            pc.CellPos = new Vector3Int(info.PosX, info.PosY, info.PosZ);
            pc.transform.rotation = Quaternion.Euler(info.RotX, info.RotY, info.RotZ);
        }
    }
    public void Add(int id, GameObject go)
    {
        mObjects.Add(id, go);
    }

    public void Remove(int id)
    {
        mObjects.Remove(id);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null)
            return;

        Remove(MyPlayer.Id);
        MyPlayer = null;
    }

    public GameObject Find(Vector3Int cellPos)
    {
        // BigO(N)
        foreach(GameObject obj in mObjects.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        // BigO(N)
        foreach (GameObject obj in mObjects.Values)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        mObjects.Clear();
    }
}
