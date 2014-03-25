using UnityEngine;
using System.Collections;

public class DestoryPrefab : MonoBehaviour {

    public static void DestoryMenuChoocePrefab()
    {
        DestoryParent(InitPrefab.MenuChooceObject);
    }

    public static void DestoryRegPrefab()
    {
        DestoryParent(InitPrefab.RegObject);
        //DestoryZhezhaoPrefab();
    }

    public static void DestoryZhezhaoPrefab()
    {
        DestoryParent(InitPrefab.ZhezhaoObject);
    }

    public static void DestoryXiaoxiPrefab()
    {
        DestoryParent(InitPrefab.XiaoxiObject);
        DestoryZhezhaoPrefab();
    }

    public static void DestoryGamePrefab()
    {
        DestoryParent(InitPrefab.GameObject);
    }

    public static void DestoryTckPrefab()
    {
        DestoryParent(InitPrefab.TckObject);
        DestoryZhezhaoPrefab();
    }
    /// <summary>
    /// 销毁对象的父物体
    /// </summary>
    /// <param name="gb"></param>
    private static void DestoryParent(GameObject gb)
    {
        if (gb != null)
        {
            Destroy(gb);
        }
    }
}
