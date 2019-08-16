/****************************************************
	文件：PECommon.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/05 22:37   	
	功能：客户端服务端共用工具类
*****************************************************/
using PENet;
using PEProtocol;

public enum LogType
{
    Log = 0,
    Warn = 1,
    Error = 2,
    Info = 3
}

public class PECommon
{
    public static void Log(string msg = "", LogType tp = LogType.Log)
    {
        LogLevel lv = (LogLevel)tp;
        PETool.LogMsg(msg, lv);
    }

    /// <summary>
    /// 计算战斗力
    /// </summary>
    public static int GetFightByProps(PlayerData playerData)
    {
        return playerData.lv * 100 + playerData.ad + playerData.ap + playerData.addef + playerData.apdef;
    }

    /// <summary>
    /// 计算各个等级对应的最高体力值
    /// </summary>
    public static int GetStaminaLimitByLv(int lv)
    {
        return ((lv  - 1)/10) * 150  + 150;
    }

    public static int GetHpByLv(int lv)
    {
        return 2000 + (lv - 1) * 500;
    }

    public static void UpdateExp(PlayerData pd, int exp)
    {
        int curLv = pd.lv;
        int curHp = pd.hp;
        int curExp = pd.exp;
        int addRestExp = exp;
        while (true)
        {
            int needExp = GetExpLimitByLv(curLv) - curExp;
            if (addRestExp >= needExp)
            {
                curLv += 1;
                curExp = 0;
                curHp += 500;
                addRestExp -= needExp;
            }
            else
            {
                pd.lv = curLv;
                pd.exp = curExp + addRestExp;
                pd.hp = curHp;
                break;
            }
        }
    }

    public static int GetExpLimitByLv(int lv)
    {
        return 100 * lv * lv;
    }


    public const int StaminaAddSpace = 5;//单位：分钟
    public const int StaminaAddCount = 2;
}

