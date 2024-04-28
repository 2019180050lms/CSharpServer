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
            MyPlayer.PosInfo = info.PosInfo;
        }
        else
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Player");
            go.name = info.Name;
            mObjects.Add(info.PlayerId, go);

            PlayerController pc = go.GetComponent<PlayerController>();
            pc.Id = info.PlayerId;
            pc.PosInfo = info.PosInfo;
        }
    }

    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;

        mObjects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null)
            return;

        Remove(MyPlayer.Id);
        MyPlayer = null;
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        mObjects.TryGetValue(id, out go);
        return go;
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
        foreach (GameObject obj in mObjects.Values)
            Managers.Resource.Destroy(obj);

        mObjects.Clear();
    }
}
