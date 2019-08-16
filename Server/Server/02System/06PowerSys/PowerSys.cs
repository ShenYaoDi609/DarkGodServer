/****************************************************
	文件：PowerSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/20 10:12   	
	功能：体力恢复系统
*****************************************************/
using System;
using PEProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PowerSys
{
    private static PowerSys _instance = null;
    public static PowerSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PowerSys();
            }
            return _instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;

        TimerSvc.Instance.AddTimeTask(CalcStaminaGrow, PECommon.StaminaAddSpace,PETimeUnit.Minute,0);
        PECommon.Log("PowerSys Init Done");
    }

    private void CalcStaminaGrow(int timeID)
    {
        //计算体力增长
        PECommon.Log("All  Onlion Player Calc Power Increase....");
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshStamina
        };
        msg.pshStamina = new PshStamina();

        //更新在线玩家体力
        Dictionary<ServerSession, PlayerData> onLineSessionDic = cacheSvc.GetOnlineSessionDic();
        foreach(ServerSession session in onLineSessionDic.Keys)
        {
            PlayerData playerData = onLineSessionDic[session];
            int maxStamina = PECommon.GetStaminaLimitByLv(playerData.lv);
            if(playerData != null)
            {
                if(playerData.stamina < maxStamina)
                {
                    playerData.stamina  += PECommon.StaminaAddCount;
                    playerData.time = TimerSvc.Instance.GetNowTime();
                    if (playerData.stamina > maxStamina)
                    {
                        playerData.stamina = maxStamina;
                    }
                }

                if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    msg.pshStamina.stamina = playerData.stamina;
                    session.SendMsg(msg);
                }
            }
            else
            {
                msg.err = (int)ErrorCode.ServerDataError;
            }            
        }

        //更新离线玩家体力

        

    }

}

