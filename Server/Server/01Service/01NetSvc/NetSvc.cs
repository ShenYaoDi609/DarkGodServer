/****************************************************
	文件：NetSvc.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/05 18:55   	
	功能：网络服务
*****************************************************/
using PENet;
using PEProtocol;
using System.Collections.Generic;

public class PackMsg
{
    public ServerSession session;
    public GameMsg msg;
    public PackMsg(ServerSession _session,GameMsg _msg)
    {
        session = _session;
        msg = _msg;
    }
}


public class NetSvc
{
    private static NetSvc _instance = null;
    public static NetSvc Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NetSvc();
            }
            return _instance;
        }
    }

    //锁
    private static readonly string obj = "lock";
    private Queue<PackMsg> msgPackQueue = new Queue<PackMsg>();

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.srvIP, SrvCfg.srvPort);

        PECommon.Log("NetSvc Init Done.");
    }

    //加入消息队列
    public void AddMsgQue(ServerSession session,GameMsg msg)
    {
        lock(obj)
        {
            msgPackQueue.Enqueue(new PackMsg (session,msg));
        }
    }

    public void Update()
    {
        if(msgPackQueue.Count > 0)
        {
            PECommon.Log("msgPackCount:" + msgPackQueue.Count);
            lock(obj)
            {
                PackMsg pack = msgPackQueue.Dequeue();
                HandOutMsg(pack);
            }
        }
    }

    //消息包处理
    public void HandOutMsg(PackMsg pack)
    {
        switch((CMD)pack.msg.cmd)
        {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            case CMD.ReqTask:
                AutoGuideSys.Instance.ReqTask(pack);
                break;
            case CMD.ReqStrong:
                StrongSys.Instance.ReqStrong(pack);
                break;
            case CMD.SndChat:
                ChatSys.Instance.SndChat(pack);
                break;
            case CMD.ReqPurchase:
                PurchaseSys.Instance.ReqPurchase(pack);
                break;
            case CMD.ReqTakeTaskReward:
                TaskSys.Instance.ReqTakeTaskReward(pack);
                break;
            case CMD.ReqFBFight:
                FuBenSys.Instance.ReqFBFight(pack);
                break;
            case CMD.ReqFBFightEnd:
                FuBenSys.Instance.ReqFBFightEnd(pack);
                break;
        }
    }

    
}

