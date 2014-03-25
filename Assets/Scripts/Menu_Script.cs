using UnityEngine;
using System.Collections;
using System.Threading;

public class Menu_Script : MonoBehaviour
{
    void Start()
    {
        cam = GameObject.Find("Camera");
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("name")))
        {
            string _name = PlayerPrefs.GetString("name");
            GameObject.Find("txtIntro").GetComponent<UILabel>().text = "尊敬的" + _name .Split('|')[1]+ "欢迎您";
            if(Globle.onlineInfo.totalNum!=0)
                GameObject.Find("totalNum").GetComponent<UILabel>().text=Globle.onlineInfo.totalNum.ToString();
        }
    }

    /// <summary>
    /// 显示注册
    /// </summary>
    void Show_Reg()
    {
        print("reg");
        InitPrefab.InitRegPrefab();
    }
    XiaoxiScript xiaoxiScript = new XiaoxiScript();
    /// <summary>
    /// 注册的用户名
    /// </summary>
    public UILabel txtName;
    /// <summary>
    /// 执行注册操作
    /// </summary>
    private void Do_Reg()
    {
        DestoryPrefab.DestoryRegPrefab();
        GameStruct.RegInfo regInfo = new GameStruct.RegInfo(0);
        regInfo.name = System.Text.Encoding.UTF8.GetBytes(txtName.text);
        print(txtName.text);
        GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(21, 31, MyDataFormatChange.StructToBytes(regInfo));
        xiaoxiScript.Init_Show_XunhuanMsg("注册中");
        Globle.IsStartWork = 1;
       
    }
    RaycastHit hitt = new RaycastHit();

    public GameObject cam;
    void OnMouseDown()
    {

        Ray ray = cam.camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hitt, 100);
        if (null != hitt.transform)
        {
            print(hitt.point);//鼠标点击的坐标
        }
    }
    /// <summary>
    /// 注册关闭按钮
    /// </summary>
    private void Do_Reg_Cancel()
    {
        DestoryPrefab.DestoryRegPrefab();
        DestoryPrefab.DestoryZhezhaoPrefab();
    }

    /// <summary>
    /// 执行加入游戏操作
    /// </summary>
    private void Do_Join()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("name")))
        {
            xiaoxiScript.Init_Show_XunhuanMsg("进入中");
            Globle.IsStartWork = 1;
            string _name = PlayerPrefs.GetString("name");
            print(_name);
            string name = _name.Split('|')[1];
            int id = int.Parse(_name.Split('|')[0]);
            GameStruct.JoinInfo joinInfo = new GameStruct.JoinInfo();
            joinInfo.name = System.Text.Encoding.UTF8.GetBytes(name);
            joinInfo.id = id;
            GameObject.Find("Updata").GetComponent<NetWorkToServer>().SendMessageEX1(20, 11, MyDataFormatChange.StructToBytes(joinInfo));
        }
    }
}
