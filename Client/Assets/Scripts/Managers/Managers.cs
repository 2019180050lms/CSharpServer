using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Contents
    MapManager mMap = new MapManager();
    ObjectManager mObj = new ObjectManager();
    NetworkManager mNetwork = new NetworkManager();

    public static MapManager Map { get { return Instance.mMap; } }
    public static ObjectManager Object { get { return Instance.mObj; } }
    public static NetworkManager Network { get { return Instance.mNetwork; } }
	#endregion

	#region Core
	DataManager mData = new DataManager();
    PoolManager mPool = new PoolManager();
    ResourceManager mResource = new ResourceManager();
    SceneManagerEx mScene = new SceneManagerEx();
    SoundManager mSound = new SoundManager();
    UIManager mUI = new UIManager();

    public static DataManager Data { get { return Instance.mData; } }
    public static PoolManager Pool { get { return Instance.mPool; } }
    public static ResourceManager Resource { get { return Instance.mResource; } }
    public static SceneManagerEx Scene { get { return Instance.mScene; } }
    public static SoundManager Sound { get { return Instance.mSound; } }
    public static UIManager UI { get { return Instance.mUI; } }
	#endregion

	void Start()
    {
        Init();
	}

    void Update()
    {
        mNetwork.Update();
    }

    static void Init()
    {
        if (s_instance == null)
        {
			GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance.mNetwork.Init();
            s_instance.mData.Init();
            s_instance.mPool.Init();
            s_instance.mSound.Init();
        }		
	}

    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
