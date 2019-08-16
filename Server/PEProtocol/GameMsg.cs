/****************************************************
	文件：Class1.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/05 19:08   	
	功能：网络通信协议(客户端服务端共用)
*****************************************************/
using System;
using PENet;

namespace PEProtocol
{
    [Serializable]
    public class GameMsg:PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;
        public ReqRename reqRename;
        public RspRename rspRename;
        public ReqTask reqTask;
        public RspTask rspTask;
        public ReqStrong reqStrong;
        public RspStrong rspStrong;
        public SndChat sndChat;
        public PshChat pshChat;
        public ReqPurchase reqPurchase;
        public RspPurchase rspPurchase;
        public PshStamina pshStamina;
        public ReqTakeTaskReward reqTakeTaskReward;
        public RspTakeTaskReward rspTakeTaskReward;
        public PshTaskProgs pshTaskProgs;
        public RspFBFight rspFBFight;
        public ReqFBFight reqFBFight;
        public ReqFBFightEnd reqFBFightEnd;
        public RspFBFightEnd rspFBFightEnd;
    }

    #region 登录相关
    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pwd;
    }

    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
        //TODO
    }

    [Serializable]
    public class ReqRename
    {
        public string name;
    }

    [Serializable]
    public class RspRename
    {
        public string name;
    }


    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int stamina;
        public int coin;
        public int diamond;
        public int crystal;
        public int hp;
        public int energy;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge;//闪避概率
        public int pierce;//穿透比率
        public int critical;//暴击 概率
        public int taskid;//当前任务id
        //各个装备的星级
        public int[] strongArr;
        public long time;
        //任务管理
        public string[] taskArr;
        //当前关卡
        public int fuben;
        //TODO
    }

    #endregion

    #region 任务相关
    [Serializable]
    public class ReqTask
    {
        public int taskID;
    }

    [Serializable]
    public class RspTask
    {
        public int taskID;
        public int coin;
        public int exp;
        public int lv;
        public int hp;
    }

    #endregion

    #region 强化相关
    [Serializable]
    public class ReqStrong
    {
        public int pos;
        public int starlv;
    }

    [Serializable]
    public class RspStrong
    {
        public int pos;
        public int starlv;
        public int addhp;
        public int addhurt;
        public int adddef;
        public int minlv;
        public int coin;
        public int crystal;
        public int[] strongArr;
    }
    #endregion

    #region 聊天相关
    [Serializable]
    public class SndChat
    {
        public string msg;
    }

    [Serializable]
    public class PshChat
    {
        public string name;
        public string msg;
    }
    #endregion

    #region 购买相关
    [Serializable]
    public class ReqPurchase
    {
        public int buyType;
        public int costDiamond;
    }

    [Serializable]
    public class RspPurchase
    {
        public int buyType;
        public int diamond;
        public int coin;
        public int stamina;
    }

    #endregion

    #region 体力回复相关
    [Serializable]
    public class PshStamina
    {
        public int stamina;
    }
    #endregion

    #region 任务奖励
    [Serializable]
    public class ReqTakeTaskReward
    {
        public int rid;
    }

    [Serializable]
    public class RspTakeTaskReward
    {
        public int exp;
        public int lv;
        public int hp;
        public int coin;
        public string[] taskArr; 
    }

    [Serializable]
    public class PshTaskProgs
    {
        public string[] taskArr;
    }
    #endregion

    #region 副本战斗相关
    [Serializable]
    public class ReqFBFight
    {
        public int fbid;
    }
    [Serializable]
    public class RspFBFight
    {
        public int fbid;
        public int stamina;
    }

    [Serializable]
    public class ReqFBFightEnd
    {
        public bool isWin;
        public int fbid;
        public int restHp;
        public int costTime;
    }

    [Serializable]
    public class RspFBFightEnd
    {
        public bool isWin;
        public int fbid;
        public int restHp;
        public int costTime;
        //副本奖励
        public int exp;
        public int lv;
        public int coin;
        public int crystal;
        public int fuben;
    }
    #endregion

    public enum CMD
    {
        None = 0,
        //登录相关
        ReqLogin = 101,
        RspLogin = 102,
        ReqRename = 103,
        RspRename = 104,
        //任务系统相关
        ReqTask = 201,
        RspTask = 202,
        //强化系统相关
        ReqStrong = 203,
        RspStrong = 204,
        //聊天系统相关
        SndChat = 205,
        PshChat = 206,
        //购买系统相关
        ReqPurchase = 207,
        RspPurchase = 208,
        //体力系统相关
        PshStamina = 209,
        //任务奖励相关
        ReqTakeTaskReward = 210,
        RspTakeTaskReward = 211,

        PshTaskProgs = 212,
        //副本战斗相关
        ReqFBFight = 301,
        RspFBFight = 302,
        ReqFBFightEnd = 303,
        RspFBFFightEnd = 304,
    }

    public enum ErrorCode
    {
        None = 0,   
        ServerDataError,//服务器数据异常
        AcctIsOnLine,  //账号已上线
        WrongPwd,   //密码错误
        NameIsExist,    //名字已经存在 
        UpdateDBError,  //更新数据库出错
        LackLevel,//等级不够
        LackCoin,//金币不够
        LackCrystal,//水晶不够
        ClientDataError,
        LackStamia,
    }

    public class SrvCfg
    {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }
}
