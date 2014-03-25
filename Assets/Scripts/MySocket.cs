using UnityEngine;
using System.Collections;
using System;
using System.Configuration;

using System.Xml;
using System.IO;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class MySocket 
{
    public delegate void CallBack_Connect(bool success,Erro_Socket error,string exception);
    public delegate void CallBack_Send(bool success, Erro_Socket error, string exception);
    public delegate void CallBack_Receive(bool success, Erro_Socket error, string exception,byte[] byte_Message,string str_Message);
    public delegate void CallBack_Disconnect(bool success, Erro_Socket error, string exception);

    /// <summary>
    /// 连接回调
    /// </summary>
    public CallBack_Connect      callBack_Connect;
    /// <summary>
    /// fa送回调
    /// </summary>
    public CallBack_Send         callBack_Send;
    /// <summary>
    /// 获取消息回调
    /// </summary>
    public CallBack_Receive      callBack_Receive;
    /// <summary>
    /// 断开连接回调
    /// </summary>
    public CallBack_Disconnect   callBack_Disconnect;

    private Erro_Socket error_Socket = 0;

    private int receiveDataLength = 0;//当前接收长度
    private int allDataLengh = 0; //这类数据总长度
    private byte[] receiveDataAL;//数据拼接用

    public enum Erro_Socket
    {
        SUCCESS = 0,                       //成功
        TIMEOUT = 1,                     //超时
        SOCKET_NULL = 2,                //套接字为空
        SOCKET_UNCONNECT = 3,               //套接字未连接
        CONNECT_UNSUCCESS_UNKNOW = 4,       //连接失败未知错误
        CONNECT_CONNECED_ERROR = 5,        //重复连接错误
        SEND_UNSUCCESS_UNKNOW = 6,          //fa送失败未知错误
        RECEIVE_UNSUCCESS_UNKNOW = 7,       //收消息未知错误
        DISCONNECT_UNSUCCESS_UNKNOW = 8,        //断开连接未知错误
    }

    private int responseCount = 0;
    private int sendCount = 0;
    private Socket clientSocket;
    private string addressIP;
    private int port;
    private int countTmp = 0;
    private bool isReceiveHalfHead = false;


    public string AddressIP { get { return addressIP; } }
    public int Port { get { return port; } }


    public bool IsConnect()
    { 
        bool isConnect=false;
        if(clientSocket==null)
        {
            return isConnect;
        }
        return clientSocket.Connected;
    }

    /// <summary>
    /// 建立连接
    /// </summary>
    /// <param name="ip"> IP</param>
    /// <param name="port">port</param>
    /// <param name="callback_Connect">连接回调</param>
    /// <param name="callBack_Receive">接收消息回调</param>
    public void Async_Connect(string ip, int port, CallBack_Connect callback_Connect, CallBack_Receive callBack_Receive)
    {
        error_Socket = Erro_Socket.SUCCESS;
        this.callBack_Connect = callback_Connect;
        this.callBack_Receive = callBack_Receive;
        if (clientSocket != null && clientSocket.Connected)
        {
            this.callBack_Connect(false, Erro_Socket.CONNECT_CONNECED_ERROR, "");//重复连接错误
        }
        else if (clientSocket == null || !clientSocket.Connected)
        {
            addressIP = ip;
            this.port = port;
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);
            //Debug.Log("Creating socket");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           // clientSocket.Blocking = true;
            IAsyncResult asyncConnect = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(connectCallback), clientSocket);
            writeDot(asyncConnect);


        }
    
    }

    //public void NetSock()
    //{
        
    //}
   /// <summary>
   /// 连接回调
   /// </summary>
   /// <param name="asyncConnect"></param>
    private void connectCallback(IAsyncResult asyncConnect)
    {
        try 
        {
          //  Debug.Log("Beginning connectCallback");
            Socket clientSocket = (Socket)asyncConnect.AsyncState;
            clientSocket.EndConnect(asyncConnect);
           // Debug.Log("Operation Completed");
            // arriving here means the operation completed
            // (asyncConnect.IsCompleted = true) but not necessarily successfully
            if (clientSocket.Connected == false)
            {
                //Debug.Log("连接失败1");
                error_Socket = Erro_Socket.CONNECT_UNSUCCESS_UNKNOW;
                
                return;
            }
            else
            {
                //Debug.Log("连接成功");
                Receive();//开始接受消息
            }
            if (callBack_Connect != null)//回调
            {
                callBack_Connect(clientSocket.Connected,error_Socket, "");
            }
            
        }catch(Exception e)
        {
            //Debug.Log(e.ToString());
            if (callBack_Connect != null)//回调
            {
                //Debug.Log("连接失败2");
                callBack_Connect(false, Erro_Socket.CONNECT_UNSUCCESS_UNKNOW,e.ToString());

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                clientSocket = null;

            }
        }
     
    }
    /// <summary>
    /// fa送消息EX
    /// </summary>
    /// <param name="type1"></param>
    /// <param name="type2"></param>
    /// <param name="type3"></param>
    /// <param name="player"></param>
    /// <param name="data"></param>
    /// <param name="callBack_Send"></param>
    public void SendMessageEX(int type1,int type2,byte[] data,CallBack_Send callBack_Send)
    {
        //Debug.Log("bbbbbbbbbbbb");
        Async_Send(NetWorkToServer.PackageMyMessage(type1, type2, data), callBack_Send);
    }

    /// <summary>

    ///// <summary>
    ///// fa送string 类型数据
    ///// </summary>
    ///// <param name="message"></param>
    ///// <param name="callBack_Send"></param>
    //public void Async_Send(string message, CallBack_Send callBack_Send)
    //{
    //    error_Socket = Erro_Socket.SUCCESS;
    //    this.callBack_Send = callBack_Send;
    //   //message += "\r\n";

    //    //Encoding GB2312 = Encoding.GetEncoding("gb2312");
    //    //Encoding Unicode = Encoding.Unicode;
    //    //byte[] data = Unicode.GetBytes(message);
    //    //byte[] sendBuffer = System.Text.Encoding.Convert(Unicode, GB2312, data);

    //    byte[] sendBuffer = MyDataFormatChange.stringToByte(message);
      
    //    //Debug.Log(sendBuffer.Length.ToString());

    //    if (clientSocket == null)
    //    {
    //        error_Socket = Erro_Socket.SOCKET_NULL;

    //    }
    //    else if (!clientSocket.Connected)
    //    {
    //        error_Socket = Erro_Socket.SOCKET_UNCONNECT;
    //    }
    //    else
    //    {
    //        if (!FishGameManager.GetAllGameManager().isSendMessage)
    //            return;
    //        IAsyncResult asyncSend = clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
    //        // clientSocket.Send(sendBuffer);
    //        writeDot(asyncSend);
    //    }

    //}

    /// <summary>
    /// fa送byte[]类型数据
    /// </summary>
    /// <param name="sendBuffer"></param>
    /// <param name="callBack_Send"></param>
    public void Async_Send(byte[] sendBuffer, CallBack_Send callBack_Send)
    {
        error_Socket = Erro_Socket.SUCCESS;
        this.callBack_Send = callBack_Send;
        if (clientSocket == null)
        {
            error_Socket = Erro_Socket.SOCKET_NULL;
            Debug.Log("套接字为空，fa送失败");
            callBack_Send(false, error_Socket, "");
        }
        else if (!clientSocket.Connected)
        {
            error_Socket = Erro_Socket.SOCKET_UNCONNECT;
            Debug.Log("未连接，fa送失败");
            callBack_Send(false, error_Socket, "");
        }
        else
        {
            //MyDataFormatChange.WriteToFile("c:\\Send\\" + sendCount,sendBuffer);
            //sendCount++;
            IAsyncResult asyncSend = clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
            writeDot(asyncSend);
        }
    }



    ///// <summary>
    ///// fa送string 类型数据
    ///// </summary>
    ///// <param name="message"></param>
    ///// <param name="callBack_Send"></param>
    //public void Async_SendWithHead(string message,int mainID, int assistantID, int handleCode, int check, CallBack_Send callBack_Send)
    //{
    //    error_Socket = Erro_Socket.SUCCESS;
    //    this.callBack_Send = callBack_Send;
    //    Encoding GB2312 = Encoding.GetEncoding("gb2312");
    //    Encoding Unicode = Encoding.Unicode;
    //    byte[] data = Unicode.GetBytes(message);
    //    byte[] sendBuffer = System.Text.Encoding.Convert(Unicode, GB2312, data);



    //    //Debug.Log(sendBuffer.Length.ToString());

    //    if (clientSocket == null)
    //    {
    //        error_Socket = Erro_Socket.SOCKET_NULL;

    //    }
    //    else if (!clientSocket.Connected)
    //    {
    //        error_Socket = Erro_Socket.SOCKET_UNCONNECT;
    //    }
    //    else
    //    {
    //        sendBuffer=GetAllMessageWithHead(sendBuffer, mainID, assistantID, handleCode, check);//加头
    //        //FileStream fs = new FileStream("c:\\sss.txt",FileMode.Create);
    //        //fs.Write(sendBuffer,0,sendBuffer.Length);
    //        //fs.Close();
    //        IAsyncResult asyncSend = clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
    //        // clientSocket.Send(sendBuffer);
    //        writeDot(asyncSend);
    //    }

    //}

    ///// <summary>
    ///// fa送byte[]类型数据
    ///// </summary>
    ///// <param name="sendBuffer"></param>
    ///// <param name="callBack_Send"></param>
    //public void Async_SendWithHead(byte[] sendBuffer,int mainID, int assistantID, int handleCode, int check, CallBack_Send callBack_Send)
    //{
    //    error_Socket = Erro_Socket.SUCCESS;
    //    this.callBack_Send = callBack_Send;
    //    if (clientSocket == null)
    //    {
    //        error_Socket = Erro_Socket.SOCKET_NULL;
    //        Debug.Log("套接字为空，fa送失败");
    //        callBack_Send(false, error_Socket, "");
    //    }
    //    else if (!clientSocket.Connected)
    //    {
    //        error_Socket = Erro_Socket.SOCKET_UNCONNECT;
    //        Debug.Log("未连接，fa送失败");
    //        callBack_Send(false, error_Socket, "");
    //    }
    //    else
    //    {
    //       // GetAllMessageWithHead(sendBuffer, mainID, assistantID, handleCode, check);//加头
    //        IAsyncResult asyncSend = clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
    //        Debug.Log("Sending data.");
    //        writeDot(asyncSend);
    //    }
    //}

    ///// <summary>
    ///// 加头的完整数据
    ///// </summary>
    ///// <param name="message"></param>
    ///// <param name="mainID"></param>
    ///// <param name="assistantID"></param>
    ///// <param name="handleCode"></param>
    ///// <param name="check"></param>
    ///// <returns></returns>
    //public static string GetAllMessageWithHead(string message, int mainID, int assistantID, int handleCode, int check)
    //{
    //    string getMessageWithHead = "";
    //    int uMessageSize = message.Length + 20;
    //    Debug.Log("message.Length:" + message.Length);
    //    Encoding Unicode = Encoding.Unicode;

    //    byte[] bytetmp = System.BitConverter.GetBytes(uMessageSize);
    //    string strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(mainID);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(assistantID);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(handleCode);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(check);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    //getMessageWithHead +=message;
    //    //Debug.Log(getMessageWithHead);
    //    return getMessageWithHead;
    //}

    ///// <summary>
    ///// 加头的完整数据
    ///// </summary>
    ///// <param name="message"></param>
    ///// <param name="mainID"></param>
    ///// <param name="assistantID"></param>
    ///// <param name="handleCode"></param>
    ///// <param name="check"></param>
    ///// <returns></returns>
    //public static byte[] GetAllMessageWithHead(byte[] message, int mainID, int assistantID, int handleCode, int check)
    //{
    //    Encoding Unicode = Encoding.Unicode;
    //    string getMessageWithHead = "";//Unicode.GetString(message, 0, message.Length);
    //    int uMessageSize = message.Length + 20;

    //    byte[] bytetmp = System.BitConverter.GetBytes(uMessageSize);
    //    string strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(mainID);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(assistantID);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(handleCode);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    bytetmp = System.BitConverter.GetBytes(check);
    //    strtmp = Unicode.GetString(bytetmp, 0, bytetmp.Length);
    //    getMessageWithHead += strtmp;

    //    getMessageWithHead += Unicode.GetString(message, 0, message.Length); ;
    //    byte[] data = Unicode.GetBytes(getMessageWithHead);
    //    return data;



    //}


    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="callBack_Disconnect"></param>
    public void Async_Disconnect(CallBack_Disconnect callBack_Disconnect)
    {
        try
        {
            error_Socket = Erro_Socket.SUCCESS;
            this.callBack_Disconnect = callBack_Disconnect;
            if (clientSocket == null)
            {
                error_Socket = Erro_Socket.SOCKET_NULL;
                callBack_Disconnect(true, error_Socket, "");
                //Debug.Log("套接字为空，断开失败");
            }
            else if (!clientSocket.Connected)
            {
                error_Socket = Erro_Socket.SOCKET_UNCONNECT;
                callBack_Disconnect(true, error_Socket, "");
                //Debug.Log("已经断开连接，断开失败");
            }
            else
            {
                IAsyncResult asyncDisconnect = clientSocket.BeginDisconnect(false, new AsyncCallback(disconnectCallback), clientSocket);
                
                writeDot(asyncDisconnect);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
     

    }

    /// <summary>
    /// 断开连接回调
    /// </summary>
    /// <param name="asyncDisconnect"></param>
    private  void disconnectCallback(IAsyncResult asyncDisconnect)
    {
        Debug.Log("disconnectCallback");
        try
        {
            Socket clientSocket = (Socket)asyncDisconnect.AsyncState;
            clientSocket.EndDisconnect(asyncDisconnect);
            clientSocket.Close();
            clientSocket = null;
          
            if (callBack_Disconnect != null)//回调
            {
                //Debug.Log("成功断开连接");
               // NetworkDataManager.isExistGame = true;
               // Application.Quit();
                //callBack_Disconnect(true, error_Socket,"");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            if (callBack_Disconnect != null)//回调
            {
                //Debug.Log("断开失败");
                callBack_Disconnect(true,Erro_Socket.DISCONNECT_UNSUCCESS_UNKNOW,e.ToString());
            }
           
        }
    }

    /// <summary>
    /// fa送消息回调
    /// </summary>
    /// <param name="asyncSend"></param>
    private void sendCallback(IAsyncResult asyncSend)
    {

        try
        {
         //    Debug.Log("sendCallback");
            Socket clientSocket = (Socket)asyncSend.AsyncState;
            int bytesSent = clientSocket.EndSend(asyncSend);
            //Debug.Log(bytesSent + "bytes sent.");
            if (callBack_Send != null)//回调
            {
                callBack_Send(true, error_Socket,"");
            }
        
        }catch(Exception e)
        {
            //Debug.Log(e.ToString());
            if (callBack_Send != null)//回调
            {
                callBack_Send(true, Erro_Socket.SEND_UNSUCCESS_UNKNOW,e.ToString());
            }
        }
      
    }

    /// <summary>
    /// 获取消息
    /// </summary>
    /// <param name="callBack_Receive"></param>
    public void Receive()
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            StateObject stateObject = new StateObject(2048, clientSocket);
            clientSocket.BeginReceive(stateObject.sBuffer, 0, stateObject.sBuffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), stateObject);
        }
      
    }

    ///// <summary>
    ///// 获取消息回调
    ///// </summary>
    ///// <param name="asyncReceive"></param>
    //private void receiveCallback(IAsyncResult asyncReceive)
    //{
    //    Debug.Log("receiveCallBack");
    //    try
    //    {
    //        StateObject stateObject = (StateObject)asyncReceive.AsyncState;
    //        if (stateObject.sSocket==null)
    //        {
    //            callBack_Receive(false, Erro_Socket.SOCKET_NULL,"",null,"");
    //            //Debug.Log("套接字为空，获得消息失败" );
    //            return;
    //        }else if(!stateObject.sSocket.Connected)
    //        {
    //            callBack_Receive(false, Erro_Socket.SOCKET_UNCONNECT,"",null,"");//连接断开
    //            return;
    //        }
    //        //    return;
    //        int length = stateObject.sSocket.EndReceive(asyncReceive); //返回实际长度length


    //        FileStream fs = new FileStream("c:\\Response\\Response"+responseCount+".txt", FileMode.Append);
    //        fs.Write(stateObject.sBuffer, 0, length);
    //        Debug.Log("Length:" + length);
    //        ++responseCount;
    //        fs.Close();
    //       // Debug.Log("获得消息:" + receiveString);
    //        if (callBack_Receive != null)//回调
    //        {
    //            //callBack_Receive(true, error_Socket, "", receiveByte, receiveString);
    //            callBack_Receive(true, error_Socket, "", stateObject.sBuffer, MyDataFormatChange.ByteToString(stateObject.sBuffer));
    //        }
    //    }catch (Exception e)
    //    {
    //        Debug.Log(e.ToString());
    //        if (callBack_Receive != null)//回调
    //        {
    //            callBack_Receive(false, Erro_Socket.RECEIVE_UNSUCCESS_UNKNOW,e.ToString(),null,"");
    //        }
    //    }
    //    Receive();
    //}

    /// <summary>
    /// 获取消息回调
    /// </summary>
    /// <param name="asyncReceive"></param>
    private void receiveCallback(IAsyncResult asyncReceive)
    {
       //Debug.Log("receiveCallBack");
        try
        {
            StateObject stateObject = (StateObject)asyncReceive.AsyncState;
            if (stateObject.sSocket == null)
            {

                callBack_Receive(false, Erro_Socket.SOCKET_NULL, "", null, "");
                //Debug.Log("套接字为空，获得消息失败" );
                return;
            }
            else if (!stateObject.sSocket.Connected)
            {
                callBack_Receive(false, Erro_Socket.SOCKET_UNCONNECT, "", null, "");//连接断开
                Debug.Log("连接断开");
                return;
            }
            //    return;
            int length = stateObject.sSocket.EndReceive(asyncReceive); //返回实际获得数据长度length
            Debug.Log("aaaa" + length);
            if(length==0)
                return;

            //if (length > 0)
            //{
            //    Debug.Log("responseCount" + responseCount);
            //    FileStream fs = new FileStream("c:\\Response\\Response" + (responseCount++), FileMode.Append);
            //    fs.Write(stateObject.sBuffer, 0, length);
            //    fs.Close();
            //}

            byte[] byteTMP = new byte[length];
            MemCpy(byteTMP, stateObject.sBuffer, 0, length);

            if (isReceiveHalfHead)//头只有一半
            {
                isReceiveHalfHead = false;
                MemCpy(receiveDataAL, byteTMP, receiveDataLength);
                receiveDataLength += length;

            }
            else if (receiveDataLength == 0)//第一次接收该类型
            {
                DismantleDataWithHead(byteTMP);
            }
            else//上次没接收完(zhong途，没头)
            {
                DismantleDataWithoutHead(byteTMP);
            }

        }
        catch (Exception e)
        {
            // Debug.Log(e.ToString());
            if (callBack_Receive != null)//回调
            {
                callBack_Receive(false, Erro_Socket.RECEIVE_UNSUCCESS_UNKNOW, e.ToString(), null, "");
            }
        }
        Receive();
    }

    public static void MemCpy(byte[] des, byte[] sou, int start)
    {
        if (des==null || sou==null)
        {
            return;
        }
        for (int a = 0; a < sou.Length; ++a)
        {
            if (des.Length > start + a && sou.Length > a)
            { des[start + a] = sou[a]; }

        }
    }

    public static void MemCpy(byte[] des, byte[] sou, int start,int length)
    {
        for (int a = 0; a < length; ++a)
        {
            des[start + a] = sou[a];
        }
    }

    public static void MemCpy(byte[] des, byte[] sou, int start1, int start2, int length)
    {
        if(des.Length-start1<length||sou.Length-start2<length)
        {
            return;
        }
        for (int a = 0; a < length; ++a)
        {
            des[start1 + a] = sou[start2+a];
        }
    }

    void DismantleDataWithHead(byte[] allData)
    {
        try
        {
            if (allData.Length < 4)//头只有一半
            {
                isReceiveHalfHead = true;
                //receiveDataAL.Add(allData);
                MemCpy(receiveDataAL, allData, receiveDataLength);
                receiveDataLength += allData.Length;
            }
            else
            {

                byte[] dataLenghbyte = new byte[4];
                for (int a = 0; a < 4; ++a)
                {
                    dataLenghbyte[a] = allData[a];
                }

                allDataLengh = MyDataFormatChange.byteToInt(dataLenghbyte);
                //if (allDataLengh == 0)
                //{
                //    receiveDataAL = null;
                //    receiveDataLength = 0;
                //    return;
                //}
                receiveDataAL = new byte[allDataLengh];
                if (allData.Length == allDataLengh)//一次刚好接完
                {
                    //receiveDataAL.Add(allData);
                    MemCpy(receiveDataAL, allData, receiveDataLength);
                    receiveDataLength += allData.Length;
                    ReceiveMessageOver();
                }
                else if (allData.Length > allDataLengh)//本次接完还有别的数据
                {
                    MemCpy(receiveDataAL, allData, receiveDataLength, allDataLengh);
                    receiveDataLength += allData.Length;
                    int remainDataLength = allData.Length - allDataLengh;
                    ReceiveMessageOver();


                    byte[] data = new byte[remainDataLength];
                    for (int a = 0; a < remainDataLength; ++a)
                    {
                        data[a] = allData[a + (allData.Length - remainDataLength)];
                    }
                    DismantleDataWithHead(data);
                }
                else //本次接不完
                {
                    MemCpy(receiveDataAL, allData, receiveDataLength);
                    receiveDataLength += allData.Length;
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        
       
    }


    void DismantleDataWithoutHead(byte[] allData)
    {
        try 
        {
            if (receiveDataLength + allData.Length == allDataLengh)//本次刚好接完
            {
                // receiveDataAL.Add(allData);
                MemCpy(receiveDataAL, allData, receiveDataLength);
                receiveDataLength += allData.Length;
                ReceiveMessageOver();
            }
            else if (receiveDataLength + allData.Length > allDataLengh)///本次接完还有别的数据
            {
                int getDataLength = allDataLengh - receiveDataLength;//接收的数据
                byte[] data = new byte[getDataLength];

                for (int a = 0; a < getDataLength; ++a)
                {
                    data[a] = allData[a];
                }
                //receiveDataAL.Add(data); 
                MemCpy(receiveDataAL, data, receiveDataLength);
                receiveDataLength += data.Length;
                ReceiveMessageOver();


                data = new byte[allData.Length - getDataLength];//剩余数据
                for (int a = 0; a < allData.Length - getDataLength; ++a)
                {
                    data[a] = allData[a + getDataLength];
                }
                DismantleDataWithHead(data);
            }
            else//仍然没接完
            {
                // receiveDataAL.Add(allData);
                MemCpy(receiveDataAL, allData, receiveDataLength);
                receiveDataLength += allData.Length;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
      
    }

    //byte[] DataArrayListToByteArray ()
    //{
    //     int dataLength = 0;
    //    for (int a = 0; a < receiveDataAL.Length; ++a)
    //    {
    //        dataLength += ((byte[])receiveDataAL[a]).Length;
    //    }
    //    byte[] alldataByte = new byte[dataLength];
    //    int point=0;
    //    for (int a = 0; a < receiveDataAL.Count;++a )
    //    {
    //        for (int b = 0; b < ((byte[])receiveDataAL[a]).Length;++b )
    //        {
    //            alldataByte[point++] = ((byte[])receiveDataAL[a])[b];
    //        }
    //    }
    //    return alldataByte;
    //}

    void ReceiveMessageOver()
    {
        if (receiveDataAL.Length == 0)
            return;

        //FileStream fs = new FileStream("c:\\Response\\Response" + countTmp + ".txt", FileMode.Create);
        //fs.Write(receiveDataAL, 0, receiveDataAL.Length);
        //fs.Close();
        countTmp++;
        // Debug.Log("获得消息:" + receiveString);
        if (callBack_Receive != null)//回调
        {
            //callBack_Receive(true, error_Socket, "", receiveByte, receiveString);
            callBack_Receive(true, error_Socket, "", receiveDataAL, MyDataFormatChange.ByteToString(receiveDataAL));
        }
        receiveDataLength = 0;
        receiveDataAL = null;
        allDataLengh = 0;
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        if (clientSocket != null && clientSocket.Connected)
        {
            IAsyncResult asyncDisconnect = clientSocket.BeginDisconnect(false, new AsyncCallback(disconnectCallback), clientSocket);
            writeDot(asyncDisconnect);
        }
    }

    /// <summary>
    /// 超时检测
    /// </summary>
    /// <param name="ar"></param>
    /// <returns></returns>
    internal  bool writeDot(IAsyncResult ar)
    {
        int i = 0;
        while (ar.IsCompleted == false)
        {
            if (i++ > 20)
            {
               // Debug.Log("Timed out.");
                error_Socket = Erro_Socket.TIMEOUT;
                return false;
            }
            //Debug.Log(".");
            Thread.Sleep(100);
        }
        return true;
    }

    class StateObject
    {
        internal byte[] sBuffer;
        internal Socket sSocket;
        internal StateObject(int size, Socket sock)
        {
            sBuffer = new byte[size];
            sSocket = sock;
        }
    }
}
