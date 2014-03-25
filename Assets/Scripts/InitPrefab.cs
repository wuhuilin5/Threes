using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InitPrefab:MonoBehaviour{

    /// <summary>
    /// 文件根路径
    /// </summary>
    private static string path="Prefabs/";

    #region 选择界面的物体对象的初始化
    /// <summary>
    /// 选择界面的物体对象
    /// </summary>
    public static GameObject MenuChooceObject;
    /// <summary>
    /// 初始化选择对象
    /// </summary>
    public static void InitMenuChoocePrefab()
    {
        if (MenuChooceObject == null)
        {
            MenuChooceObject = InitParent(GameObject.Find("Anchor"), path + "Menu_Chooce");
        }
    } 
    #endregion

    #region 遮罩界面的物体对象的初始化
    /// <summary>
    /// 遮罩界面的物体对象
    /// </summary>
    public static GameObject ZhezhaoObject;
    /// <summary>
    /// 初始化遮罩对象
    /// </summary>
    public static void InitZhezhaoPrefab()
    {
        if (ZhezhaoObject == null)
        {
            ZhezhaoObject = InitParent(null, path + "Zhezhao");
        }
    }
    #endregion

    #region 注册界面的物体对象的初始化
    /// <summary>
    /// 注册界面的物体对象
    /// </summary>
    public static GameObject RegObject;
    /// <summary>
    /// 注册对象初始化
    /// </summary>
    public static void InitRegPrefab()
    {
        if (RegObject == null)
        {
            RegObject = InitParent(GameObject.Find("Anchor"), path + "Menu_Reg");
            InitZhezhaoPrefab();
        }
    }
    #endregion

    #region 消息界面的物体对象的初始化
    /// <summary>
    /// 消息界面的物体对象
    /// </summary>
    public static GameObject XiaoxiObject;
    /// <summary>
    /// 消息对象初始化
    /// </summary>
    public static void InitXiaoxiPrefab()
    {
        if (XiaoxiObject == null)
        {
            XiaoxiObject = InitParent(GameObject.Find("Anchor"), path + "Show_Msg");
            InitZhezhaoPrefab();
        }
    }
    #endregion

    #region 游戏界面的物体对象的初始化
    /// <summary>
    /// 消息界面的物体对象
    /// </summary>
    public static GameObject GameObject;
    /// <summary>
    /// 消息对象初始化
    /// </summary>
    public static void InitGamePrefab()
    {
        if (GameObject == null)
        {
            GameObject = InitParent(GameObject.Find("Anchor"), path + "Menu_Game");
        }
    }
    #endregion

    #region 游戏界面的物体对象的初始化
    /// <summary>
    /// 消息界面的物体对象
    /// </summary>
    public static GameObject TckObject;
    /// <summary>
    /// 消息对象初始化
    /// </summary>
    public static void InitTckPrefab()
    {
        if (TckObject == null)
        {
            TckObject = InitParent(GameObject.Find("Anchor"), path + "Show_Tck");
            InitZhezhaoPrefab();
        }
    }
    #endregion

    #region 黑子界面的物体对象的初始化
    /// <summary>
    /// 黑子界面的物体对象
    /// </summary>
    public static List<GameObject> BlackObject=new List<GameObject>();
    /// <summary>
    /// 黑子对象初始化
    /// </summary>
    public static void InitBlackPrefab(Vector3 vect)
    {
        GameObject gb = InitParent(GameObject.Find("Menu_Game(Clone)"), path + "black");
        gb.transform.localPosition = vect;
        gb.transform.localScale = new Vector3(0.06073928f, 0.06073928f, 0.06073928f);
        //BlackObject.Add(gb);
        Globle.lsQizi.Add(gb);
    }
    #endregion

    #region 白子界面的物体对象的初始化
    /// <summary>
    /// 白子界面的物体对象
    /// </summary>
    public static List<GameObject> WhiteObject=new List<GameObject>();
    /// <summary>
    /// 白子对象初始化
    /// </summary>
    public static void InitWhitePrefab(Vector3 vect)
    {
        GameObject gb = InitParent(GameObject.Find("Menu_Game(Clone)"), path + "white");
        gb.transform.localPosition = vect;
        gb.transform.localScale = new Vector3(0.06073928f, 0.06073928f, 0.06073928f);
        //WhiteObject.Add(gb);
        Globle.lsQizi.Add(gb);
    }
    #endregion
    private static GameObject InitParent(GameObject parent,string _path)
    {
        GameObject gb = (GameObject)Instantiate(Resources.Load(_path));
        if (parent != null)
        {
            gb.transform.parent = parent.transform;
            gb.transform.localScale = new Vector3(1, 1, 1);
            gb.transform.localPosition = new Vector3(0, 0, 0);
        }
        return gb;
    }
}
