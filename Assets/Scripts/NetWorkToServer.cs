using UnityEngine;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections.Generic;

public class NetWorkToServer : MonoBehaviour {

    public static bool isExistGame = false;
    private Thread thread2;
    private string ip;
    private int port;
    public static MySocket mySocket;
    private static Thread thread;
    public static ArrayList get_MessageDataPoolAL;
    public static ArrayList send_MessageDataPoolAL;
    private Hashtable packageCount;
    private Hashtable packageData;
    void Update()
    {
        if (get_MessageDataPoolAL != null && get_MessageDataPoolAL.Count != 0)
        {
            try
            {
                AnalyseData((byte[])get_MessageDataPoolAL[0]);

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            get_MessageDataPoolAL.RemoveAt(0);
        }
    }
    public enum MSG_TYPE1
    {
        //IPC_SUB_PUREMSG				1
        //IPC_SUB_SYSTEMMSG			2
        MSG_IPC_SYSTEMMSG = 18,
        MSG_WZQ_GAME_SYSTEM = 20,//五子棋的游戏信息
        MSG_WZQ_INFO_SYSTEM = 21,//五子棋的游戏信息
    }
    /// <summary>
    /// 服务器系统
    /// </summary>
    public enum MSG_TYPE2_1
    {
        ASS_WZQ_JOIN = 11,//五子棋请求加入游戏信息
        ASS_WZQ_EXIT = 12,//五子棋请求离开游戏信息
        ASS_WZQ_GAMING = 13,//游戏过程的消息
        ASS_WZQ_JIESU = 14,//游戏结束的消息
        ASS_WZQ_READY = 15,//五子棋准备信息
        ASS_WZQ_START = 16,//五子棋的开始信息
        ASS_WZQ_RENSHU=17,//请求认输
        ASS_WZQ_HUIQI=18,//悔棋消息
        ASS_WZQ_HUIQIRESULT = 19,//悔棋结果
        TZ_WZQ_JOIN=21,//接受客户端的通知消息

        ASS_WZQ_REG=31,//五子棋的注册
        ASS_WZQ_ONLINE = 32,//在线人数
    }

    void Awake()
    {

        mySocket = new MySocket();
        send_MessageDataPoolAL = new ArrayList();
        packageData = new Hashtable();
        packageCount = new Hashtable();

    }
    public void Connect()
    {
        thread2 = new Thread(AysConnect);
        thread2.Start();


    }

    public void ConnectEX(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        thread2 = new Thread(AysConnect);
        thread2.Start();


    }
    void AysConnect()
    {
        mySocket.Async_Connect(this.ip, this.port, CallBack_Connect, CallBack_Receive);
    }

    void CallBack_Connect(bool s, MySocket.Erro_Socket errorcode, string exception)
    {
        if (errorcode == MySocket.Erro_Socket.SUCCESS)
        {
            thread = new Thread(LoopSendMessage);
            thread.Start();
            Debug.Log("connOK");
        }
        else
        {
            Debug.Log("CallBack_Connect" + s + errorcode.ToString() + exception);
        }

    }

    void CallBack_Send(bool s, MySocket.Erro_Socket errorcode, string exception)
    {
        if (errorcode != MySocket.Erro_Socket.SUCCESS)
        {
            Debug.Log("CallBack_Send" + s + errorcode.ToString() + exception);
        }
    }


    void LoopSendMessage()
    {
        while (mySocket != null && mySocket.IsConnect())
        {
            if (send_MessageDataPoolAL != null && send_MessageDataPoolAL.Count != 0)
            {
                mySocket.Async_Send((byte[])send_MessageDataPoolAL[0], CallBack_Send);
                send_MessageDataPoolAL.RemoveAt(0);
            }
            Thread.Sleep(20);
        }
    }

    void OnDestory()
    {
        Disconnect();
    }
    void CallBack_DisConnect(bool s, MySocket.Erro_Socket errorcode, string exception)
    {
        // 
        if (thread != null && errorcode == MySocket.Erro_Socket.SUCCESS)
        {
            thread.Abort();
        }
        else
        {
            Debug.Log("CallBack_DisConnect" + s + errorcode.ToString() + exception);
        }

    }

    void CallBack_Receive(bool s, MySocket.Erro_Socket errorcode, string exception, byte[] byteMessage, string strMessage)
    {
        if (errorcode != MySocket.Erro_Socket.SUCCESS)
        {
            Debug.Log("CallBack_Receive" + s + errorcode.ToString() + exception + strMessage);
        }
        else
        {
            PutGetMessageToPool(byteMessage);
        }

    }
    public static void SendMessageEX(int type1, int type2, byte[] data)
    {
        if (mySocket != null)
        {
            PutMessageToPool(type1, type2, data);
        }

    }

    public void SendMessageEX1(int type1, int type2, byte[] data)
    {
        if (mySocket != null)
        {
            PutMessageToPool(type1, type2, data);
        }

    }



    /// <summary>
    /// 收到消息入池
    /// </summary>
    /// <param name="get_Message"></param>
    public static void PutGetMessageToPool(int type1, int type2, byte[] message)
    {
        byte[] get_Message = new byte[20 + message.Length];

        MySocket.MemCpy(get_Message, MyDataFormatChange.intToByte(get_Message.Length), 0, 4);
        MySocket.MemCpy(get_Message, MyDataFormatChange.intToByte(type1), 4, 4);
        MySocket.MemCpy(get_Message, MyDataFormatChange.intToByte(type2), 8, 4);
        MySocket.MemCpy(get_Message, message, 20, message.Length);
        if (get_MessageDataPoolAL == null)
        {
            get_MessageDataPoolAL = new ArrayList();
        }
        get_MessageDataPoolAL.Add(get_Message);
    }

    /// <summary>
    /// 收到消息入池
    /// </summary>
    /// <param name="get_Message"></param>
    void PutGetMessageToPool(byte[] get_Message)
    {
        if (get_MessageDataPoolAL == null)
        {
            get_MessageDataPoolAL = new ArrayList();
        }
        get_MessageDataPoolAL.Add(get_Message);
    }

    /// <summary>
    /// fa送消息入池
    /// </summary>
    /// <param name="type1"></param>
    /// <param name="type2"></param>
    /// <param name="data"></param>
    public static void PutMessageToPool(int type1, int type2, byte[] data)
    {
        if (send_MessageDataPoolAL == null)
        {
            send_MessageDataPoolAL = new ArrayList();
        }
        send_MessageDataPoolAL.Add(PackageMyMessage(type1, type2, data));
        Debug.Log("poolCall:"+send_MessageDataPoolAL.Count);
    }

    public static byte[] PackageMyMessage(int Type1, int Type2, byte[] data)
    {

        return PackageMyMessage(Type1, Type2, 0, 0, data);
    }

     /// <summary>
    /// 打包消息 
    /// </summary>
    /// <param name="Type1"></param>
    /// <param name="Type2"></param>
    /// <param name="uHandleCode"></param>
    /// <param name="uCheck"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] PackageMyMessage(int Type1, int Type2, int uHandleCode, int uCheck, byte[] data)
    {
        byte[] messageData;
        messageData = MyDataFormatChange.ConnectBytes(MyDataFormatChange.intToByte(0)/*打包消息总长度（4字节）*/, MyDataFormatChange.intToByte(Type1));
        messageData = MyDataFormatChange.ConnectBytes(messageData, MyDataFormatChange.intToByte(Type2));
        messageData = MyDataFormatChange.ConnectBytes(messageData, MyDataFormatChange.intToByte(uHandleCode));
        messageData = MyDataFormatChange.ConnectBytes(messageData, MyDataFormatChange.intToByte(uCheck));

        if (data != null)
        {

            messageData = MyDataFormatChange.ConnectBytes(messageData, data);
        }

        byte[] allDataLengthByte = new byte[4];
        allDataLengthByte = MyDataFormatChange.intToByte(messageData.Length);
        for (int a = 0; a < 4; ++a)
        {
            messageData[a] = allDataLengthByte[a];
        }
        return messageData;
    }

    /// <summary>
    /// 数据处理
    /// </summary>
    /// <param name="byteMessage"></param>
    void AnalyseData(byte[] byteMessage)
    {
        byte[] byteTMP = new byte[4];
        MySocket.MemCpy(byteTMP, byteMessage, 0, 4);
        int dataLength = MyDataFormatChange.byteToInt(byteTMP);//文件长度

        MySocket.MemCpy(byteTMP, byteMessage, 0, 4, 4);
        int dataType1 = MyDataFormatChange.byteToInt(byteTMP);//文件主ID1

        MySocket.MemCpy(byteTMP, byteMessage, 0, 8, 4);
        int dataType2 = MyDataFormatChange.byteToInt(byteTMP);//文件主ID2

        MySocket.MemCpy(byteTMP, byteMessage, 0, 12, 4);
        int codeID = MyDataFormatChange.byteToInt(byteTMP);//文件主codeID




        byte[] messageData = new byte[dataLength - 20];
        MySocket.MemCpy(messageData, byteMessage, 0, 20, dataLength - 20);
        if (codeID > 1)
        {
            if (!packageCount.ContainsKey((int)dataType1 + "_" + (int)dataType2))//首次，最大包数
            {
                packageCount.Add((int)dataType1 + "_" + (int)dataType2, codeID);
            }

            if (!packageData.ContainsKey((int)dataType1 + "_" + (int)dataType2 + "_" + codeID))
            {
                packageData.Add((int)dataType1 + "_" + (int)dataType2 + "_" + codeID, messageData);
            }
            return;
        }
        else if (codeID == 1)
        {
            if (!packageData.ContainsKey((int)dataType1 + "_" + (int)dataType2 + "_" + codeID))
            {
                packageData.Add((int)dataType1 + "_" + (int)dataType2 + "_" + codeID, messageData);
            }
            int count = 0;
            if (packageCount.ContainsKey((int)dataType1 + "_" + (int)dataType2))//首次，最大包数
            {
                count = (int)packageCount[(int)dataType1 + "_" + (int)dataType2];
            }
            int offset = 0;
            int messageLength = 0;

            for (int a = 0; a < count; ++a)
            {
                messageLength += ((byte[])packageData[(int)dataType1 + "_" + (int)dataType2 + "_" + (a + 1)]).Length;
            }
            if (messageLength != 0)
            {

                messageData = new byte[messageLength];
                for (int a = 0; a < count; ++a)
                {
                    byte[] byteTmp = (byte[])packageData[(int)dataType1 + "_" + (int)dataType2 + "_" + (count - a)];
                    MySocket.MemCpy(messageData, byteTmp, offset);
                    offset += byteTmp.Length;
                }
                packageCount.Clear();
                packageData.Clear();
            }


        }

        //IsLoginOk = (dataLength - 20);
        Debug.Log("ID1:  " + (int)dataType1 + "        ID2:  " + (int)dataType2 + "      dataLength:" + (dataLength - 20));
        switch (dataType1)
        {
            /////////////////////////////模拟服务器消息/////////////////////////////////////////////
            ///五子棋游戏数据
            case (int)NetWorkToServer.MSG_TYPE1.MSG_WZQ_GAME_SYSTEM:
                switch (dataType2)
                {

                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_JOIN://加入
                        MSG_Analyse_JOIN(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_EXIT://离开

                        MSG_Analyse_EXIT(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_GAMING://游戏
 
                        MSG_Analyse_GAME(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_JIESU://结束消息
                        //MSG_Analyse_PersonInfo(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_READY://准备消息

                        MSG_Analyse_READY(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_START://开始消息
                        MSG_Analyse_START(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.TZ_WZQ_JOIN://通知消息
                        MSG_Analyse_TZJOIN(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_RENSHU://认输消息
                        MSG_Analyse_RENSHU(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_HUIQI://悔棋请求
                        MSG_Analyse_HUIQI(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_HUIQIRESULT://开始结果
                        MSG_Analyse_HUIQIRESULT(messageData);
                        break;
                }
                break;

            case (int)NetWorkToServer.MSG_TYPE1.MSG_WZQ_INFO_SYSTEM:
                switch (dataType2)
                {
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_REG://注册
                        MSG_Analyse_REG(messageData);
                        break;
                    case (int)NetWorkToServer.MSG_TYPE2_1.ASS_WZQ_ONLINE://注册
                        MSG_Analyse_ONLINE(messageData);
                        break;
                }
                break;

        }

    }

    /// <summary>
    /// 在线人数信息
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_ONLINE(byte[] messageData)
    {
        Globle.onlineInfo = (GameStruct.OnLineInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.OnLineInfo(), messageData);
        GameObject.Find("totalNum").GetComponent<UILabel>().text=Globle.onlineInfo.totalNum.ToString();
        DestoryPrefab.DestoryXiaoxiPrefab();
    }

    /// <summary>
    /// 移除空的字符串
    /// </summary>
    /// <param name="str">要移除的字符串</param>
    /// <returns>移除后的字符串</returns>
    public static string RemoveEmptyChar(string str)
    {
        int index = str.IndexOf("\0");
        if(index!=-1)
            str = str.Remove(index);
        return str;
    }
    /// <summary>
    /// 悔棋结果
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_HUIQIRESULT(byte[] messageData)
    {
        Globle.huiqiInfo = (GameStruct.HuiqiInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.HuiqiInfo(), messageData);
        if (Globle.huiqiInfo.isOk == 3)
        {
            List<string> lsHui = Globle.dicStep[Globle.joinInfo.deskId];
            int _x1 = int.Parse(lsHui[lsHui.Count - 1].Split('|')[0]);
            int _y1 = int.Parse(lsHui[lsHui.Count - 1].Split('|')[1]);

            int _x2 = int.Parse(lsHui[lsHui.Count - 2].Split('|')[0]);
            int _y2 = int.Parse(lsHui[lsHui.Count - 2].Split('|')[1]);
            print("(" + _x1 + "," + _y1 + ")");
            print("(" + _x2 + "," + _y2 + ")");
            int[,] value = Globle.allPoint;
            print(value.Length);
            value[_x1, _y1] = 0;
            value[_x2, _y2] = 0;//悔了2步棋
            print(Globle.lsQizi.Count);
            print(Globle.lsQizi[Globle.lsQizi.Count - 2]);
            Destroy(Globle.lsQizi[Globle.lsQizi.Count - 2]);
            print(Globle.lsQizi[Globle.lsQizi.Count - 1]);
            Destroy(Globle.lsQizi[Globle.lsQizi.Count - 1]);
            Globle.lsQizi.RemoveAt(Globle.lsQizi.Count - 1);
            Globle.lsQizi.RemoveAt(Globle.lsQizi.Count - 1);
            XiaoxiScript.Init_Show_JiandanMsg("悔棋成功");
        }
        else if (Globle.huiqiInfo.isOk == 4)
        {
            XiaoxiScript.Init_Show_JiandanMsg("悔棋失败");
        }
    }
    /// <summary>
    /// 悔棋请求
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_HUIQI(byte[] messageData) 
    {
        Globle.huiqiInfo = (GameStruct.HuiqiInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.HuiqiInfo(), messageData);
        if (Globle.huiqiInfo.playNum != Globle.joinInfo.selfNum)
        {
            Globle.tckInfo = 4;//是否同意
            InitPrefab.InitTckPrefab();
        }
    }

    /// <summary>
    /// 通知玩家在线信息
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_TZJOIN(byte[] messageData)
    {
        Globle.joinResult = (GameStruct.JoinResultInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.JoinResultInfo(0), messageData);
        string player1 = GetString(Globle.joinResult.player1Name);
        string player2 = GetString(Globle.joinResult.player2Name);
        print("player1:"+player1+"|player2:"+player2);
        GameObject.Find("player1").GetComponent<UILabel>().text =RemoveEmptyChar( player1);
        GameObject.Find("player2").GetComponent<UILabel>().text =RemoveEmptyChar( player2);
        GameObject.Find("t1gold").GetComponent<UILabel>().text = Globle.joinResult.player1Gold.ToString();
        GameObject.Find("t2gold").GetComponent<UILabel>().text = Globle.joinResult.player2Gold.ToString();
        GameObject.Find("t1lose").GetComponent<UILabel>().text = Globle.joinResult.player1loseNum.ToString();
        GameObject.Find("t2lose").GetComponent<UILabel>().text = Globle.joinResult.player2loseNum.ToString();
        GameObject.Find("t1win").GetComponent<UILabel>().text = Globle.joinResult.player1winNum.ToString();
        GameObject.Find("t2win").GetComponent<UILabel>().text = Globle.joinResult.player2winNum.ToString();
        GameObject.Find("t1total").GetComponent<UILabel>().text = Globle.joinResult.player1totalNum.ToString();
        GameObject.Find("t2total").GetComponent<UILabel>().text = Globle.joinResult.player2totalNum.ToString();
       
    }

    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_REG(byte[] messageData)
    {
        Globle.IsStartWork = 0;//工作完成
        Globle.regInfo = (GameStruct.RegInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.RegInfo(0), messageData);
        print(System.Text.Encoding.UTF8.GetString(Globle.regInfo.name));
        if (Globle.regInfo.id == 0)
        {
            print("注册失败");
            XiaoxiScript.Init_Show_JiandanMsg("注册失败,用户名已经存在");
        }
        else
        {
            string name = RemoveEmptyChar(GetString(Globle.regInfo.name));
            PlayerPrefs.SetString("name", Globle.regInfo.id + "|" + name);
            XiaoxiScript.Init_Show_JiandanMsg("注册成功");
            GameObject.Find("txtIntro").GetComponent<UILabel>().text = "尊敬的" + name + "欢迎您";
        }
    }

    /// <summary>
    /// 准备消息
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_READY(byte[] messageData)
    {
        Globle.readyResult = (GameStruct.ReadyResultInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.ReadyResultInfo(), messageData);
        if (Globle.readyResult.player1Ready == 1)
        {
            GameObject.Find("player1Ready").GetComponent<UILabel>().text = "准备";
        }
        else if (Globle.readyResult.player2Ready == 1)
        {
            GameObject.Find("player2Ready").GetComponent<UILabel>().text = "准备";
        }
    }

    /// <summary>
    /// 离开消息
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_EXIT(byte[] messageData)
    {
        Globle.exitInfo = (GameStruct.ExitInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.ExitInfo(), messageData);
        
        if (Globle.exitInfo.exitNum == Globle.joinInfo.selfNum)//表示是自己离开
        {
            
            Invoke("Delay_Exite", 0.5f);
            GameObject.Find("player1Ready").GetComponent<UILabel>().text = "未准备";
            GameObject.Find("player1").GetComponent<UILabel>().text = "等待中";
            GameObject.Find("player2Ready").GetComponent<UILabel>().text = "未准备";
            GameObject.Find("player2").GetComponent<UILabel>().text = "等待中";
            print("我离开了游戏");
        }
        else//表示是别人离开
        {
            if (Globle.exitInfo.exitNum == 1)//1号离开
            {
                GameObject.Find("player1Ready").GetComponent<UILabel>().text = "未准备";
                GameObject.Find("player1").GetComponent<UILabel>().text = "等待中";
            }
            else//2号离开
            {
                GameObject.Find("player2Ready").GetComponent<UILabel>().text = "未准备";
                GameObject.Find("player2").GetComponent<UILabel>().text = "等待中";
            }
        }
    }

    private void Delay_Exite()
    {
        DestoryPrefab.DestoryGamePrefab();
        InitPrefab.InitMenuChoocePrefab();
        DestoryPrefab.DestoryXiaoxiPrefab();
    }
    /// <summary>
    /// 用户加入游戏的处理方法
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_JOIN(byte[] messageData)
    {
        Globle.IsStartWork = 0;//工作完成
        if (messageData.Length == 0)
        {
            XiaoxiScript.Init_Show_JiandanMsg("您正在游戏中，不能重复加入");
            return;
        }
        Globle.joinInfo = (GameStruct.JoinInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.JoinInfo(0), messageData);
        DestoryPrefab.DestoryXiaoxiPrefab();
        DestoryPrefab.DestoryMenuChoocePrefab();
        InitPrefab.InitGamePrefab();
        Invoke("Join_Delay",0.2f);   
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="messageData"></param>
    public void MSG_Analyse_START(byte[] messageData)
    {
        Globle.startInfo = (GameStruct.StartInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.StartInfo(), messageData);
        ResetOperationTime();
        DaoJishi();
        if (Globle.startInfo.isStart == 1)
        {
            XiaoxiScript.Init_Show_JiandanMsg("游戏开始");
            Globle.zhuanNum = Globle.startInfo.zhuangNum;
            if (Globle.zhuanNum == 1)
            {
                GameObject.Find("blackPlayer").GetComponent<UILabel>().text = GetString(Globle.joinResult.player1Name);
                GameObject.Find("whitePlayer").GetComponent<UILabel>().text = GetString(Globle.joinResult.player2Name);
            }
            else
            {
                GameObject.Find("blackPlayer").GetComponent<UILabel>().text = GetString(Globle.joinResult.player2Name);
                GameObject.Find("whitePlayer").GetComponent<UILabel>().text = GetString(Globle.joinResult.player1Name);
            }
            GameObject.Find("player1Ready").GetComponent<UILabel>().text = "游戏中";
            GameObject.Find("player2Ready").GetComponent<UILabel>().text = "游戏中";
        }
    }

    public void MSG_Analyse_RENSHU(byte[] messageData)
    {
        Globle.renshuInfo = (GameStruct.RenshuInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.RenshuInfo(), messageData);
        Globle.gameInfo.winNum=((Globle.renshuInfo.playNum == 1) ? 2 : 1);
        Jieshu(Globle.gameInfo.winNum, Globle.renshuInfo.deskId);
        //GameObject.Find("player1Ready").GetComponent<UILabel>().text = "未准备";
        //GameObject.Find("player2Ready").GetComponent<UILabel>().text = "未准备";
        //XiaoxiScript.Init_Show_JiandanMsg("player" + ((Globle.renshuInfo.playNum == 1) ? 2 : 1) + "胜利");
        //Globle.ReadyBtn.SetActiveRecursively(true);//隐藏按钮
        //Globle.startInfo.isStart = 0;//表示游戏已经结束
        //NewGame(Globle.renshuInfo.deskId);
    }
    void Join_Delay()
    {
        print(Globle.joinInfo.playerwinNum+","+Globle.joinInfo.playertotalNum+","+Globle.joinInfo.playerloseNum);
        if (Globle.joinInfo.selfNum == 1)
        {
            GameObject.Find("player1").GetComponent<UILabel>().text =RemoveEmptyChar( GetString(Globle.joinInfo.name));
            GameObject.Find("t1gold").GetComponent<UILabel>().text = Globle.joinInfo.playerGold.ToString();
            GameObject.Find("t1lose").GetComponent<UILabel>().text = Globle.joinInfo.playerloseNum.ToString();
            GameObject.Find("t1win").GetComponent<UILabel>().text = Globle.joinInfo.playerwinNum.ToString();
            GameObject.Find("t1total").GetComponent<UILabel>().text = Globle.joinInfo.playertotalNum.ToString();
        }
        else
        {
            GameObject.Find("player2").GetComponent<UILabel>().text = RemoveEmptyChar(GetString(Globle.joinInfo.name));
            GameObject.Find("t2gold").GetComponent<UILabel>().text = Globle.joinInfo.playerGold.ToString();
            GameObject.Find("t2lose").GetComponent<UILabel>().text = Globle.joinInfo.playerloseNum.ToString();
            GameObject.Find("t2win").GetComponent<UILabel>().text = Globle.joinInfo.playerwinNum.ToString();
            GameObject.Find("t2total").GetComponent<UILabel>().text = Globle.joinInfo.playertotalNum.ToString();
        }
    }

    private void Jieshu(int winNum,int deskId)
    {
        GameObject.Find("player1Ready").GetComponent<UILabel>().text = "未准备";
        GameObject.Find("player2Ready").GetComponent<UILabel>().text = "未准备";

        print(Globle.gameInfo.winNum);
        //XiaoxiScript.Init_Show_JiandanMsg("player" + winNum + "胜利");
        Invoke("Show_Jiesuan",0.5f);

        CancelInvoke("DaoJishi");
        Globle.ReadyBtn.SetActiveRecursively(true);//隐藏按钮
        Globle.startInfo.isStart = 0;//表示游戏已经结束
        NewGame(deskId);
    }
    void Show_Jiesuan()
    {
        Globle.tckInfo = 2;
        InitPrefab.InitTckPrefab();
    }

    private void ResetOperationTime()
    {
        Globle.OperationTime = 30;//收到消息还原为默认的30秒
    }

    public void MSG_Analyse_GAME(byte[] messageData)
    {
        Globle.gameInfo = (GameStruct.GameInfo)MyDataFormatChange.BytesToStructEX(new GameStruct.GameInfo(), messageData);
        ResetOperationTime();
        float realX = -210;
        float realY = 156;
        int _x = Globle.gameInfo.content % 15;
        int _y = Globle.gameInfo.content / 15;
        float x = (_x * 30) + realX;
        float y = realY - (_y * 30);
        List<string> lsStep = null;
        if (!Globle.dicStep.ContainsKey(Globle.joinInfo.deskId))//是否包含这桌的信息
        {
            Globle.dicStep.Add(Globle.joinInfo.deskId, new List<string>());
            lsStep = Globle.dicStep[Globle.joinInfo.deskId];
        }
        else
            lsStep = Globle.dicStep[Globle.joinInfo.deskId];
        if (Globle.gameInfo.playNum == Globle.zhuanNum)
        {
            InitPrefab.InitBlackPrefab(new Vector3(x, y));
            Globle.allPoint[_x, _y] = Globle.zhuanNum;
            lsStep.Add(_x + "|" + _y + "|" + Globle.zhuanNum);
        }
        else
        {
            InitPrefab.InitWhitePrefab(new Vector3(x, y));
            Globle.allPoint[_x, _y] = ((Globle.zhuanNum == 1) ? 2 : 1);
            lsStep.Add(_x + "|" + _y + "|" + ((Globle.zhuanNum == 1) ? 2 : 1));
        }
        if (Globle.lsQizi.Count >= 2)
        {
            Globle.lsQizi[Globle.lsQizi.Count - 2].GetComponent<Animation>().Stop();
            Globle.lsQizi[Globle.lsQizi.Count - 2].transform.localScale = new Vector3(0.06073928f, 0.06073928f, 0.06073928f);
        }
            Globle.gameInfo.playNum = ((Globle.gameInfo.playNum == 1) ? 2 : 1);//切换用户来下棋
        print("winnum:" + Globle.gameInfo.winNum);
        if (Globle.gameInfo.winNum > 0)
        {
            Jieshu(Globle.gameInfo.winNum,Globle.gameInfo.deskId);
        }
    }

    /// <summary>
    /// 倒计时操作
    /// </summary>
    void DaoJishi()
    {
        GameObject.Find("daoJishi").GetComponent<UILabel>().text = Globle.OperationTime.ToString();
        Globle.OperationTime--;
        print(Globle.gameInfo.playNum==1);
        if (Globle.gameInfo.playNum==1)
        {
            GameObject.Find("left").GetComponent<UISlicedSprite>().enabled = true;
            GameObject.Find("right").GetComponent<UISlicedSprite>().enabled = false;
        }
        else
        {
            GameObject.Find("left").GetComponent<UISlicedSprite>().enabled = false;
            GameObject.Find("right").GetComponent<UISlicedSprite>().enabled = true;
        }
        if (Globle.OperationTime == -1)//表示倒计时已经到了，发送认输请求
        {
            TckScript tck = new TckScript();
            tck.btn_Renshu();
        }
        else
            Invoke("DaoJishi", 1f);
    }

    /// <summary>
    /// 用于开始一盘新的游戏
    /// </summary>
    /// <param name="_deskId"></param>
    private void NewGame(int _deskId)
    {
        Globle.joinInfo.deskId = _deskId;
        Globle.joinResult.deskId = _deskId;
        Globle.readyInfo.deskId = _deskId;
        Globle.readyResult.deskId = _deskId;
        Globle.exitInfo.deskId = _deskId;
        Globle.allPoint = new int[15, 15];
        
    }
    /// <summary>
    /// 转换字符串
    /// </summary>
    /// <param name="byt"></param>
    /// <returns></returns>
    public static string GetString(byte[] byt)
    {
        return RemoveEmptyChar(System.Text.Encoding.UTF8.GetString(byt));
    }
    /// <summary>
    /// 转换字符串
    /// </summary>
    /// <param name="byt"></param>
    /// <returns></returns>
    public static byte[] GetBytes(string str)
    {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }
   
    public void Disconnect()
    {
        mySocket.Async_Disconnect(CallBack_DisConnect);//断开连接
    }
    //public static GameStruct.SYSTEM_USERINFO personData;
    //public void MSG_Analyse_PersonInfo(byte[] messageData)
    //{
    //    print("MSG_Analyse_PersonInfo");
    //    personData = (GameStruct.SYSTEM_USERINFO)MyDataFormatChange.BytesToStructEX(new GameStruct.SYSTEM_USERINFO(0), messageData);
    //}
    /// <summary>
    /// CJX 确保结构体是否正确
    /// </summary>
    /// <param name="dataLength"></param>
    /// <param name="structLength"></param>
    /// <returns></returns>
    bool CheckStructIsRight(int dataLength, int structLength)
    {
        if (dataLength != structLength)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
