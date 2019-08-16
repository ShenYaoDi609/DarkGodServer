/****************************************************
	文件：PurchaseSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/17 9:50   	
	功能：购买业务系统
*****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PEProtocol;

public class PurchaseSys
{
    private static PurchaseSys _instance = null;
    public static PurchaseSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PurchaseSys ();
            }
            return _instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("LoginSys Init Done");
    }

    public void ReqPurchase(PackMsg pack)
    {
        ReqPurchase data = pack.msg.reqPurchase;
        ServerSession session = pack.session;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspPurchase
        };

        PlayerData playerData = cacheSvc.GetPlayerDataCache(session);
        //钻石不够
        if (playerData.diamond < data.costDiamond)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        else
        {
            playerData.diamond -= data.costDiamond;
            PshTaskProgs pshTaskPrgs = null;
            switch (data.buyType)
            {
                case 0:
                    //更新任务进度
                    pshTaskPrgs = TaskSys.Instance.CalcTaskProgs(playerData, 4);
                    playerData.stamina += 100;
                    break;
                case 1:
                    //更新任务进度
                    pshTaskPrgs = TaskSys.Instance.CalcTaskProgs(playerData, 5);
                    playerData.coin += 100;
                    break;
            }

            if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspPurchase rspPurchase = new RspPurchase
                {
                    buyType = data.buyType,
                    diamond = playerData.diamond,
                    coin = playerData.coin,
                    stamina = playerData.stamina
                };
                //并包优化
                msg.rspPurchase = rspPurchase;
                msg.pshTaskProgs = pshTaskPrgs;
            }
        }
        session.SendMsg(msg);
    }

  
}

