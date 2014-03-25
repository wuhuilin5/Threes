using UnityEngine;
using System.Collections;

public class OperationScript : MonoBehaviour {
    public UILabel txtName;
    void Btn_Start()
    {
        print("Loding");

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("name")))
        {
            string _name = PlayerPrefs.GetString("name");
            print(_name);
            string name = _name.Split('|')[1];
            int id =int.Parse( _name.Split('|')[0]);
            GameStruct.JoinInfo joinInfo = new GameStruct.JoinInfo();
            joinInfo.name = System.Text.Encoding.UTF8.GetBytes(name);
            joinInfo.id = id;
            GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 11, MyDataFormatChange.StructToBytes(joinInfo));
        }
    }

    /// <summary>
    /// 注册操作
    /// </summary>
    void Btn_Reg()
    {
        //print(PlayerPrefs.GetString("name"));
        //if (string.IsNullOrEmpty(PlayerPrefs.GetString("name")))
        //{
            GameStruct.RegInfo regInfo = new GameStruct.RegInfo(0);
            regInfo.name = System.Text.Encoding.UTF8.GetBytes(txtName.text);
            GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(21, 31, MyDataFormatChange.StructToBytes(regInfo));
        //}
        //else
        //{
        //    print("已经注册过");
        //}
    }

    /// <summary>
    /// 准备按钮的响应事件
    /// </summary>
    void Btn_Ready()
    {
        GameStruct.ReadyInfo readyInfo = new GameStruct.ReadyInfo();
        readyInfo.deskId = Globle.joinInfo.deskId;
        readyInfo.selfNum = Globle.joinInfo.selfNum;
        print("deskid:" + readyInfo.deskId + ":self:" + readyInfo.selfNum);
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 15, MyDataFormatChange.StructToBytes(readyInfo));
    }

    /// <summary>
    /// 离开游戏
    /// </summary>
    void Btn_Exit()
    {
        GameStruct.ExitInfo exitInfo = new GameStruct.ExitInfo();
        exitInfo.deskId = Globle.joinInfo.deskId;
        exitInfo.exitNum = Globle.joinInfo.selfNum;
        exitInfo.id = Globle.joinInfo.id;
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 12, MyDataFormatChange.StructToBytes(exitInfo));
    }
}
