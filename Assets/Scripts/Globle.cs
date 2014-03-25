using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Globle : MonoBehaviour {

    /// <summary>
    /// 游戏加入信息
    /// </summary>
    public static GameStruct.JoinInfo joinInfo = new GameStruct.JoinInfo();

    /// <summary>
    /// 加入游戏结果
    /// </summary>
    public static GameStruct.JoinResultInfo joinResult = new GameStruct.JoinResultInfo();

    /// <summary>
    /// 注册信息
    /// </summary>
    public static GameStruct.RegInfo regInfo = new GameStruct.RegInfo();

    /// <summary>
    /// 准备消息
    /// </summary>
    public static GameStruct.ReadyInfo readyInfo = new GameStruct.ReadyInfo();

    /// <summary>
    /// 准备结果
    /// </summary>
    public static GameStruct.ReadyResultInfo readyResult = new GameStruct.ReadyResultInfo();

    /// <summary>
    /// 离开游戏
    /// </summary>
    public static GameStruct.ExitInfo exitInfo = new GameStruct.ExitInfo();

    /// <summary>
    /// 游戏开始
    /// </summary>
    public static GameStruct.StartInfo startInfo = new GameStruct.StartInfo();

    /// <summary>
    /// 游戏过程
    /// </summary>
    public static GameStruct.GameInfo gameInfo = new GameStruct.GameInfo();

    /// <summary>
    /// 认输信息
    /// </summary>
    public static GameStruct.RenshuInfo renshuInfo = new GameStruct.RenshuInfo();

    /// <summary>
    /// 悔棋消息
    /// </summary>
    public static GameStruct.HuiqiInfo huiqiInfo = new GameStruct.HuiqiInfo();

    /// <summary>
    /// 在线人物数
    /// </summary>
    public static GameStruct.OnLineInfo onlineInfo = new GameStruct.OnLineInfo();
    /// <summary>
    /// 表示是否能显示消息框 
    /// 1表示开始工作
    /// 0表示没有工作
    /// </summary>
    public static int IsStartWork;

    /// <summary>
    /// 用来记录用户所下的棋子的位子
    /// </summary>
    public static int[,] allPoint = new int[15, 15];

    /// <summary>
    /// 庄家的ID
    /// </summary>
    public static int zhuanNum;

    /// <summary>
    /// 准备按钮对象
    /// </summary>
    public static GameObject ReadyBtn;

    /// <summary>
    /// 所有的棋子对象
    /// </summary>
    public static List<GameObject> lsQizi=new List<GameObject>();

    /// <summary>
    /// 弹出框的信息，1表示认输 2表示结束
    /// </summary>
    public static int tckInfo;

    /// <summary>
    /// 记录每一步棋子
    /// </summary>
    public static Dictionary<int, List<string>> dicStep = new Dictionary<int, List<string>>();

    /// <summary>
    /// 操作时间
    /// 默认操作时间为30秒
    /// </summary>
    public static int OperationTime=30;
}
