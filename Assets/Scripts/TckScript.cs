
using UnityEngine;
using System.Collections;

public class TckScript : MonoBehaviour {

    public UILabel title;
    public UILabel content;
	// Use this for initialization
	void Start () {
        if (Globle.tckInfo == 1)
        {
            title.text = "认输";
            content.text = "确定认输吗？";
            tck_Ok += btn_Renshu;
        }
        if (Globle.tckInfo == 2)//结算信息
        {
            Show_Jiesuan(Globle.gameInfo.winNum);
            tck_Ok += Btn_Jiesuan;
        }
        if (Globle.tckInfo == 3)//悔棋信息
        {
            Show_Huiqi();
            tck_Ok += btn_Huiqi;
        }
        if (Globle.tckInfo == 4)//同意悔棋操作
        {
            Tongyi_Huiqi();
            tck_Ok += btn_Tongyi_Huiqi;
        }
	}
    EventTckMethod tck_Ok;
    void Btn_Tck_Ok()
    {
        tck_Ok();
        DestoryPrefab.DestoryTckPrefab();
    }

    #region 请求悔棋操作
    void Show_Huiqi()
    {
        title.text = "悔棋";
        content.text = "确定要悔棋吗？";
    }

    void btn_Huiqi()
    {
        GameStruct.HuiqiInfo huiqiInfo = new GameStruct.HuiqiInfo();
        huiqiInfo.deskId = Globle.joinInfo.deskId;
        huiqiInfo.playNum = Globle.joinInfo.selfNum;
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 18, MyDataFormatChange.StructToBytes(huiqiInfo));
    } 
    #endregion

    #region 同意悔棋操作
    void Tongyi_Huiqi()
    {
        title.text = "悔棋";
        content.text = "同意对手悔棋吗？不同意请选择关闭";
    }

    /// <summary>
    /// 同意悔棋
    /// </summary>
    void btn_Tongyi_Huiqi()
    {
        Globle.huiqiInfo.isOk = 1;
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 18, MyDataFormatChange.StructToBytes(Globle.huiqiInfo));
    }
    /// <summary>
    /// 不同意悔棋
    /// </summary>
    void btn_Cancel_Huiqi()
    {
        Globle.huiqiInfo.isOk = 2;//表示不同意悔棋
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 18, MyDataFormatChange.StructToBytes(Globle.huiqiInfo));
    }
    #endregion

    /// <summary>
    /// 发送认输请求
    /// </summary>
    public void btn_Renshu()
    {
        print("认输啦。");
        GameStruct.RenshuInfo renshuInfo = new GameStruct.RenshuInfo();
        renshuInfo.playNum = Globle.joinInfo.selfNum;
        renshuInfo.deskId = Globle.joinInfo.deskId;
        print(Globle.joinInfo.selfNum+","+Globle.joinInfo.deskId);
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 17, MyDataFormatChange.StructToBytes(renshuInfo));
    }

    void Show_Jiesuan(int playerNum)
    {
        string name = "";
        title.text = "结算";
        if (playerNum == 1)
            name =NetWorkToServer.GetString(Globle.joinResult.player1Name);
        else
            name = NetWorkToServer.GetString(Globle.joinResult.player2Name);
        content.text = "玩家 " + name + " 赢了";
    }

    void Btn_Jiesuan()
    {
        GameScript.ClearQizi();
        Globle.ReadyBtn.SetActiveRecursively(false);//隐藏按钮
        GameStruct.ReadyInfo readyInfo = new GameStruct.ReadyInfo();
        readyInfo.deskId = Globle.joinInfo.deskId;
        readyInfo.selfNum = Globle.joinInfo.selfNum;
        print("deskid:" + readyInfo.deskId + ":self:" + readyInfo.selfNum);
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 15, MyDataFormatChange.StructToBytes(readyInfo));
    }

    /// <summary>
    /// 关闭弹出框
    /// </summary>
    void Btn_Tck_Close()
    {
        DestoryPrefab.DestoryTckPrefab();
    }
}
public delegate void EventTckMethod();

