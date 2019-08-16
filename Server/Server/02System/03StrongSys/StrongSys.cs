using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PEProtocol;

public class StrongSys
{
    private static StrongSys _instance = null;
    public static StrongSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StrongSys();
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
        PECommon.Log("StrongSys Init Done");
    }

    /// <summary>
    /// 回应强化请求
    /// </summary>
    public void ReqStrong(PackMsg pack)
    {
        ReqStrong data = pack.msg.reqStrong;
        ServerSession session = pack.session;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspStrong,
        };

        PlayerData playerData = cacheSvc.GetPlayerDataCache(session);
        if(playerData.strongArr[data.pos] == data.starlv)
        {
            int nextStarLv = data.starlv + 1;
            StrongCfg nextStrongCfg = cfgSvc.GetStrongCfg(data.pos, nextStarLv);

            if(playerData.lv < nextStrongCfg.minlv)
            {
                msg.err = (int)ErrorCode.LackLevel;
            }
            else if(playerData.coin < nextStrongCfg.coin)
            {
                msg.err = (int)ErrorCode.LackCoin;
            }
            else if(playerData.crystal < nextStrongCfg.crystal)
            {
                msg.err = (int)ErrorCode.LackCrystal;
            }
            else
            {
                //更新任务进度
                PshTaskProgs pshTaskProgs =  TaskSys.Instance.CalcTaskProgs(playerData, 3);

                playerData.hp += nextStrongCfg.addhp;
                playerData.ad += nextStrongCfg.addhurt;
                playerData.ap += nextStrongCfg.addhurt;
                playerData.addef += nextStrongCfg.adddef;
                playerData.apdef += nextStrongCfg.adddef;
                playerData.strongArr[data.pos] += 1;

                playerData.coin -= nextStrongCfg.coin;
                playerData.crystal -= nextStrongCfg.crystal;

                if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {

                    RspStrong rspStrong = new RspStrong
                    {
                        pos = nextStrongCfg.pos,
                        starlv = nextStrongCfg.starlv,
                        addhp = nextStrongCfg.addhp,
                        addhurt = nextStrongCfg.addhurt,
                        adddef = nextStrongCfg.adddef,
                        minlv = nextStrongCfg.minlv,
                        coin = nextStrongCfg.coin,
                        crystal = nextStrongCfg.crystal,
                        strongArr = playerData.strongArr
                    };
                    msg.rspStrong = rspStrong;
                    if(pshTaskProgs != null)
                    {
                        msg.pshTaskProgs = pshTaskProgs;
                    }
                }             
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ServerDataError;
        }
        pack.session.SendMsg(msg);
    }

}

