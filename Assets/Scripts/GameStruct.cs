using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class GameStruct : MonoBehaviour {

    /// <summary>
    /// 加入游戏的结构体
    /// 分开写只是为了结构更清楚
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct JoinInfo
    {
        public int id;
        public int deskId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] name;//姓名

        public int playerGold;
        public int playerwinNum;
        public int playerloseNum;
        public int playertotalNum;

        public int selfNum;//自己是几号玩家
        public JoinInfo(int a)
            : this()
        {
            name = new byte[20];
        }
    }

    /// <summary>
    /// 加入游戏的结果
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct JoinResultInfo
    {
        public int deskId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] player1Name;//1号用户的姓名
        public int player1Gold;
        public int player1winNum;
        public int player1loseNum;
        public int player1totalNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] player2Name;//2号用户的姓名
        public int player2Gold;
        public int player2winNum;
        public int player2loseNum;
        public int player2totalNum;
        
        public JoinResultInfo(int a)
            : this()
        {
            player1Name = new byte[20];
            player2Name = new byte[20];
        }
    }

    /// <summary>
    /// 游戏开始与结束消息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct StartInfo
    {
        /// <summary>
        /// 游戏是否开始
        /// 1表示开始 0表示结束
        /// </summary>
        public int isStart;
        /// <summary>
        /// 庄家的座位号
        /// </summary>
        public int zhuangNum;
    }

    /// <summary>
    /// 注册游戏
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RegInfo
    {
        public int id;//ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] name;//姓名

        public RegInfo(int a)
            : this()
        {
            name = new byte[20];
        }
    }
    /// <summary>
    /// 准备的结构体
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ReadyInfo
    {
        public int deskId;//桌子ID
        public int selfNum;//几号玩家：1表示1号，2表示2号
    }
    /// <summary>
    /// 准备结果的结构体
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ReadyResultInfo
    {
        public int deskId;//桌子ID

        public int player1Ready;//1号准备

        public int player2Ready;//2号准备
    }

    /// <summary>
    /// 用户离开桌子
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ExitInfo
    {
        public int id;//用户id
        public int deskId;//桌子id
        public int exitNum;//离开的是几号玩家
    }
    /// <summary>
    /// 游戏消息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct GameInfo
    {
        public int deskId;//桌子ID
        public int gameId;//下棋的ID
        public int content;//玩家落子的位置
        public int playNum;//1号还是2号下子
        public int winNum;//是1号还是2号赢了
    }

    /// <summary>
    /// 认输消息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RenshuInfo
    {
        public int playNum;//是1号还是2号认输
        public int deskId;
    }
    /// <summary>
    /// 悔棋请求
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct HuiqiInfo
    {
        public int playNum;//是1号还是2号悔棋
        public int deskId;
        /// <summary>
        /// 对手是否同意悔棋 0是默认状况，表示还没选择，1是同意 2是不同意
        /// </summary>
        public int isOk;
    }

    /// <summary>
    /// 悔棋请求
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct HuiqiResultInfo
    {
        public int playNum;//是1号还是2号悔棋
        public int deskId;
    }
    /// <summary>
    /// 在线人数消息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct OnLineInfo
    {
        /// <summary>
        /// 在线人数
        /// </summary>
        public int totalNum;
    }
    /// <summary>
    /// 获得我的消息
    /// </summary>
   [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct SYSTEM_MYINFO
    {
        public int userID;     //用户ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] UserName;//用户名
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] NiName;  //昵称
        public int Gander;     //性别


        public long UserGold; //金币
        public int Score;       //得分
        public int UserLevel;  //等级
        public int TotalNum; //总盘数
        public int WinNum;   //赢的次数
        public int worldRank; // 世界排行 
        public int ZengSongCount;//赠送金币次数
        public int BaoShiCount;//宝石的数量

        public SYSTEM_MYINFO(int a)
            : this()
        {
            UserName = new byte[20];
            NiName = new byte[20];
        }
    }

    /// <summary>
    /// 游戏信息（hu牌次数）
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct MSG_GameInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public int[] hunCount;       //各种hu牌次数
        public MSG_GameInfo(int a)
            : this()
        {
            hunCount = new int[48];
        }
    }

    /// <summary>
    /// 世界排行
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct MSG_World
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public long[] Gold;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public SelfInfo[] selfInfo;
        public MSG_World(int a)
            : this()
        {
            Gold = new long[10];
            selfInfo = new SelfInfo[10];
            for (int b = 0; b < selfInfo.Length; ++b)
            {
                selfInfo[b] = new SelfInfo(1);
            }
        }
    }

    /// <summary>
    /// 一个人的信息 昵称，金币
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct SelfInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] NikName;
        // public long Gold;
        public SelfInfo(int a)
            : this()
        {
            NikName = new byte[12];

        }
    }

    /// <summary>
    /// 系统洗牌乱序数组(136张zhong的71张)
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct MSG_CanUserShuffle_MJ_Index_Array
    {
        //谁先摸
        public int userID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 71)]
        public int[] mj_Index;
        public MSG_CanUserShuffle_MJ_Index_Array(int a)
            : this()
        {
            mj_Index = new int[71];
           
        }
    }

    /// <summary>
    /// 换手牌
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct MSG_ChangeMyHand_MJ
    {
        public int userID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public int[] mj_Index;
        public MSG_ChangeMyHand_MJ(int a)
            : this()
        {
            mj_Index = new int[13];
           
        }
    }

    /// <summary>
    /// 换手牌
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct MSG_S2C_ChangeMyHand_MJ
    {
        /// <summary>
        /// 待换手牌
        /// </summary>
        public MSG_ChangeMyHand_MJ changeMyHand;
        /// <summary>
        /// 所换牌的index
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public int[] mj_ChangeIndex;

        public MSG_S2C_ChangeMyHand_MJ(int a)
            : this()
        {
            mj_ChangeIndex = new int[13];
            changeMyHand = new MSG_ChangeMyHand_MJ(0);
        }
    }


    /// <summary>
    /// 弃牌
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct MSG_DiscardMyHand_MJ
    {
        public int userID;
        public int mj_Index;
    }
}
