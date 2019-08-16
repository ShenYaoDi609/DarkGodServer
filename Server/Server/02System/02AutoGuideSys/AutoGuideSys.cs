using PEProtocol;

public class AutoGuideSys
{
    private static AutoGuideSys _instance = null;
    public static AutoGuideSys  Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AutoGuideSys();
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
        PECommon.Log("AutoGuideSys Init Done");
    }

    /// <summary>
    /// 回应任务完成请求
    /// </summary>
    public void ReqTask(PackMsg pack)
    {
        ReqTask data = pack.msg.reqTask;
        ServerSession session = pack.session;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTask
        };

        PlayerData playerData = cacheSvc.GetPlayerDataCache(pack.session);
        AutoGuideCfg AutoGuideCfg = cfgSvc.GetAutoGuideCfg(data.taskID);
        //更新任务ID
        if (playerData.taskid == data.taskID)
        {
            PshTaskProgs pshTaskProgs = null;
            //更新任务智者点拨进度
            if(playerData.taskid == 1001)
            {
                pshTaskProgs = TaskSys.Instance.CalcTaskProgs(playerData,1);
            }
            playerData.taskid++;
            //更新玩家数据
            playerData.coin += AutoGuideCfg.coin;
            PECommon.UpdateExp(playerData, AutoGuideCfg.exp);

            if(!cacheSvc.UpdatePlayerData(playerData.id,playerData))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspTask rspTask = new RspTask
                {
                    taskID = playerData.taskid,
                    coin = playerData.coin,
                    exp = playerData.exp,
                    lv = playerData.lv,
                    hp = playerData.hp,
                };
                //发送回客户端
                msg.rspTask = rspTask;    
                if(pshTaskProgs != null)
                {
                    msg.pshTaskProgs = pshTaskProgs;
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


