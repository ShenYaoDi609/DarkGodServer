/****************************************************
	文件：FuBenSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/23 22:14   	
	功能：副本业务系统
*****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PEProtocol;

public class FuBenSys
{
    private static FuBenSys _instance = null;
    public static FuBenSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FuBenSys();
            }
            return _instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;

        PECommon.Log("FuBenSys Init Done");
    }

    public void ReqFBFight(PackMsg pack)
    {
        ReqFBFight data = pack.msg.reqFBFight;
        ServerSession session = pack.session;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspFBFight,
        };

        PlayerData playerData = cacheSvc.GetPlayerDataCache(session);
        MapCfg mapCfg = cfgSvc.GetMapCfg(data.fbid);

        if(playerData.fuben < data.fbid)
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        else if(playerData.stamina < mapCfg.costStamina)
        {
            msg.err = (int)ErrorCode.LackStamia;
        }
        else
        {
            playerData.stamina -= mapCfg.costStamina;
            if(!cacheSvc.UpdatePlayerData(playerData.id,playerData))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspFBFight rspFBFight = new RspFBFight
                {
                    fbid = data.fbid,
                    stamina = playerData.stamina,
                };
                msg.rspFBFight = rspFBFight;
            }
        }
        session.SendMsg(msg);
    }

    public void ReqFBFightEnd(PackMsg pack)
    {
        ReqFBFightEnd data = pack.msg.reqFBFightEnd;
        ServerSession session = pack.session;

        GameMsg msg = new GameMsg
        {
            cmd = (int)(CMD.RspFBFFightEnd),
        };
        PlayerData playerData = cacheSvc.GetPlayerDataCache(session);
        MapCfg mapCfg = cfgSvc.GetMapCfg(data.fbid);

        if(data.isWin)
        {
            if(data.costTime > 0 && data.restHp > 0)
            {
                TaskSys.Instance.CalcTaskProgs(playerData, 2);

                PECommon.UpdateExp(playerData, mapCfg.exp);
                playerData.coin += mapCfg.coin;
                playerData.crystal += mapCfg.crystal;
                if(data.fbid == playerData.fuben)
                {
                    playerData.fuben++;
                }

                if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    RspFBFightEnd rspFBFightEnd = new RspFBFightEnd
                    {
                        isWin = data.isWin,
                        fbid = data.fbid,
                        restHp = data.restHp,
                        costTime = data.costTime,
                        exp = playerData.exp,
                        lv = playerData.lv,
                        coin = playerData.coin,
                        crystal = playerData.crystal,
                        fuben = playerData.fuben,
                    };

                    msg.rspFBFightEnd = rspFBFightEnd;
                }
            }
            
        }
        else
        {
            msg.err = (int)ErrorCode.UpdateDBError;
        }
        session.SendMsg(msg);
    }
}

