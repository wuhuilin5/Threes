using UnityEngine;
using System.Collections;

public class UpdateScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InitPrefab.InitMenuChoocePrefab();

        //gameObject.GetComponent<NetWorkToServer>().ConnectEX("192.168.0.199", 6050);
        gameObject.GetComponent<NetWorkToServer>().ConnectEX("116.255.251.106", 6050);
        new XiaoxiScript().Init_Show_XunhuanMsg("连接中");
        //gameObject.GetComponent<NetWorkToServer>().SendMessageEX1(20, 11, null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
