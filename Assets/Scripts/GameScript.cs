using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {

    ///// <summary>
    ///// 1号玩家的名字
    ///// </summary>
    //public UILabel player1;
    ///// <summary>
    ///// 2号玩家的名字
    ///// </summary>
    //public UILabel player2;

    void Start()
    {
        Globle.ReadyBtn = GameObject.Find("btnReady");
    }

    


    /// <summary>
    /// 认输
    /// </summary>
    void Do_Renshu()
    {
        if (Globle.startInfo.isStart == 1)//表示游戏已经开始
        {
            Globle.tckInfo = 1;
            InitPrefab.InitTckPrefab();
        }
        else
        {
            XiaoxiScript.Init_Show_JiandanMsg("还没开始怎么认输啊。");
        }
    }

    void Do_Huiqi()//悔棋
    {
        if (Globle.startInfo.isStart == 1)//表示游戏已经开始
        {
            if (Globle.gameInfo.playNum == Globle.joinInfo.selfNum)
            {
                Globle.tckInfo = 3;
                InitPrefab.InitTckPrefab();
            }
            else
                XiaoxiScript.Init_Show_JiandanMsg("轮到你下时才能悔棋");
        }
        else
        {
            XiaoxiScript.Init_Show_JiandanMsg("还没开始怎么认输啊。");
        }
    }
    /// <summary>
    /// 发送准备消息
    /// </summary>
    void Do_Ready()
    {
        ClearQizi();
        Globle.ReadyBtn.SetActiveRecursively(false);//隐藏按钮
        GameStruct.ReadyInfo readyInfo = new GameStruct.ReadyInfo();
        readyInfo.deskId = Globle.joinInfo.deskId;
        readyInfo.selfNum = Globle.joinInfo.selfNum;
        print("deskid:" + readyInfo.deskId + ":self:" + readyInfo.selfNum);
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 15, MyDataFormatChange.StructToBytes(readyInfo));
    }

    /// <summary>
    /// 清除棋子
    /// </summary>
    public static void ClearQizi()
    {
        if (Globle.lsQizi.Count > 0)
        {
            for (int i = 0; i < Globle.lsQizi.Count; i++)//销毁棋子对象
            {
                Destroy(Globle.lsQizi[i]);
            }
            Globle.lsQizi = new List<GameObject>();
        }
    }

    RaycastHit hitt = new RaycastHit();
    /// <summary>
    /// 退出游戏操作
    /// </summary>
    void Do_Exit()
    {
        
        if (Globle.startInfo.isStart == 1)
        {
            XiaoxiScript.Init_Show_JiandanMsg("游戏已经开始不能退出");
            return;
        }
        new XiaoxiScript().Init_Show_XunhuanMsg("退出中");
        GameStruct.ExitInfo exitInfo = new GameStruct.ExitInfo();
        exitInfo.deskId = Globle.joinInfo.deskId;
        exitInfo.exitNum = Globle.joinInfo.selfNum;
        exitInfo.id = Globle.joinInfo.id;
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 12, MyDataFormatChange.StructToBytes(exitInfo));
    }

    /// <summary>
    /// 所有的棋子
    /// </summary>
    private int[,] allPoint = new int[15, 15];
    public int JianGe = 30;
    public float positionX = 300;
    public float positionY = 540;
    public float realX = -210;
    public float realY = 156;
    void Do_Enter_Qizi()
    {
        float _x = Input.mousePosition.x;
        float _y = Input.mousePosition.y;
        float _pointX = (_x - positionX) / 30;
        float _pointY = (positionY - _y) / 30;
        print("y:" + _y + "positiony:" + positionY);
        print("(" + _pointX + "," + _pointY + ")");
        float _tempX = _pointX - ((int)_pointX);
        float _tempY = _pointY - ((int)_pointY);
        print("(" + _tempX + "," + _tempY + ")");
        int vectX = -1;
        int vectY = -1;
        if (_tempX < 0.4)
            vectX = (int)_pointX;
        if (_tempX > 0.7)
            vectX = (int)_pointX + 1;
        if (_tempY < 0.4)
            vectY = (int)_pointY;
        if (_tempY > 0.7)
            vectY = (int)_pointY + 1;
        if (vectX != -1 && vectY != -1)
        {
            //print(Globle.startInfo.zhuangNum + ":" + Globle.gameInfo.playNum);
            if (Globle.allPoint[vectX, vectY] == 0)
            {
                if (Globle.startInfo.zhuangNum == Globle.joinInfo.selfNum || Globle.gameInfo.playNum == Globle.joinInfo.selfNum && Globle.gameInfo.winNum==0)
                {
                    Globle.startInfo.zhuangNum = 0;
                    Globle.gameInfo.playNum = 0;
                    print("该地方有棋子了");
                    //allPoint[vectX, vectY] = 1;
                    GameStruct.GameInfo gameInfo = new GameStruct.GameInfo();
                    gameInfo.deskId = Globle.joinInfo.deskId;
                    gameInfo.gameId = Globle.joinInfo.id;
                    gameInfo.playNum = Globle.joinInfo.selfNum;
                    gameInfo.content = vectX + (vectY * 15);
                    GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 13, MyDataFormatChange.StructToBytes(gameInfo));
                    print(Input.mousePosition);
                }
            }
        }
    }
    public GameObject cam;
    void OnGUI()
    {
        float PixWidth = 1024;
        float PixHeight = 768;
        float horizRatio = (float)Screen.width / (float)PixWidth;
        float vertRatio = (float)Screen.height / (float)PixHeight;
        GUI.matrix = Matrix4x4.TRS(new Vector3(0,0,0),Quaternion.identity,new Vector3(horizRatio,vertRatio,1));
        //cam = GameObject.Find("Camera");
    }

}
