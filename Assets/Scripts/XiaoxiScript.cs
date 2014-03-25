using UnityEngine;
using System.Collections;
using System.Threading;

public class XiaoxiScript : MonoBehaviour {


    public static UILabel writeMsg;
    
    /// <summary>
    /// 简单显示消息
    /// </summary>
    /// <param name="msg"></param>
    public static void Init_Show_JiandanMsg(string _msg)
    {
        InitPrefab.InitXiaoxiPrefab();
        writeMsg = GameObject.Find("Show_Msg_Label").GetComponent<UILabel>();
        writeMsg.text = _msg;
    }
    void Update()
    {
        if (Globle.IsStartWork == 0 && InitPrefab.XiaoxiObject!=null)
        {
            Invoke("DestroyXiaoxi",1f);
        }
    }
    public void DestroyXiaoxi()
    {
        DestoryPrefab.DestoryXiaoxiPrefab();
    }

    #region 用来循环显示提示的信息

    public void Init_Show_XunhuanMsg(string _msg)
    {
        InitPrefab.InitXiaoxiPrefab();
        writeMsg = GameObject.Find("Show_Msg_Label").GetComponent<UILabel>();
        msg = _msg;
        writeMsg.text = msg;
        Thread threadShow = new Thread(Show_Msg);
        threadShow.IsBackground = true;
        threadShow.Start();
    }

    string msg;

    /// <summary>
    /// 显示的label信息
    /// </summary>
    void Show_Msg()
    {
        while (Globle.IsStartWork == 1)//循环显示消息
        {
            string _temp = msg;
            for (int i = 0; i < 4; i++)
            {
                if (Globle.IsStartWork == 0)
                    break;
                _temp = _temp + ".";
                writeMsg.text = _temp;
                Thread.Sleep(500);
            }
        }
    }

    #endregion
}
