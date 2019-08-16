/****************************************************
	文件：TaskSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/21 18:54   	
	功能：任务奖励系统
*****************************************************/

using System;
using System.Collections.Generic;
using PEProtocol;

public class TaskSys
{
    private static TaskSys _instance = null;
    public static TaskSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TaskSys();
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
        
        PECommon.Log("TaskSys Init Done");
    }

    public void ReqTakeTaskReward(PackMsg pack)
    {
        ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;
        ServerSession session = pack.session;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTakeTaskReward,
        };
        int rewardID = data.rid;
        PlayerData playerData = cacheSvc.GetPlayerDataCache(session);

        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(rewardID);
        TaskRewardData trd = GetTaskRewardData(playerData, rewardID);
        
        //安全验证
        if(trd.progress == trc.count && !trd.taked)
        {
            playerData.coin += trc.coin;
            PECommon.UpdateExp(playerData, trc.exp);
            trd.taked = true;
            //更新任务进度数据
            CalcTaskArr(playerData, trd);  
            
            if(!cacheSvc.UpdatePlayerData(playerData.id,playerData))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspTakeTaskReward = new RspTakeTaskReward
                {
                    coin = playerData.coin,
                    exp = playerData.exp,
                    lv = playerData.lv,
                    hp = playerData.hp,
                    taskArr = playerData.taskArr,
                };
                session.SendMsg(msg);
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }

    }

    public TaskRewardData GetTaskRewardData(PlayerData pd,int rid)
    {
        TaskRewardData trd = null;
        for(int i = 0; i < pd.taskArr.Length; i++)
        {
            string[] taskInfo = pd.taskArr[i].Split('|');
            if(int.Parse(taskInfo[0]) == rid)
            {
                trd = new TaskRewardData()
                {
                    ID = int.Parse(taskInfo[0]),
                    progress = int.Parse(taskInfo[1]),
                    taked = taskInfo[2].Equals("1"),
                };
                break;
            }
        }
        return trd;
    }

    public void CalcTaskArr(PlayerData pd,TaskRewardData trd)
    {
        string result = trd.ID + "|" + trd.progress + "|" + (trd.taked ? 1 : 0);
        int index = -1;
        for(int i = 0; i < pd.taskArr.Length;i++)
        {
            string[] taskInfo = pd.taskArr[i].Split('|');
            if(int.Parse(taskInfo[0]) == trd.ID)
            {
                index = i;
                break;
            }
        }
        pd.taskArr[index] = result;
    }

    ////更新任务进度
    //public void CalcTaskProgs(PlayerData pd,int rid)
    //{
    //    TaskRewardData trd = GetTaskRewardData(pd, rid);
    //    TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(rid);
    //    if(trd.progress < trc.count)
    //    {
    //        trd.progress += 1;
    //    }
    //    CalcTaskArr(pd,trd);

    //    ServerSession session = cacheSvc.GetOnLineServerSession(pd.id);
    //    GameMsg msg = new GameMsg
    //    {
    //        cmd = (int)CMD.PshTaskProgs,
    //        pshTaskProgs = new PshTaskProgs
    //        {
    //            taskArr = pd.taskArr,
    //        }
    //    };
    //    if(session != null)
    //    {
    //        session.SendMsg(msg);
    //    }
    //}

    //更新任务进度
    public PshTaskProgs CalcTaskProgs(PlayerData pd, int rid)
    {
        TaskRewardData trd = GetTaskRewardData(pd, rid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(rid);
        if (trd.progress < trc.count)
        {
            trd.progress += 1;
            CalcTaskArr(pd, trd);

            return new PshTaskProgs
            {
                taskArr = pd.taskArr,
            };
        }
        else
        {
            return null;
        }
    }
}

