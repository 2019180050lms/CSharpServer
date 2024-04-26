using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    //Dictionary<int, GameObject> mObjects = new Dictionary<int, GameObject>();
    List<GameObject> mObjects = new List<GameObject>();

    public void Add(GameObject go)
    {
        mObjects.Add(go);
    }

    public void Remove(GameObject go)
    {
        mObjects.Remove(go);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        // BigO(N)
        foreach(GameObject obj in mObjects)
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
        foreach (GameObject obj in mObjects)
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
