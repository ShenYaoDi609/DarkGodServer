/****************************************************
	文件：ChatSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/16 19:52   	
	功能：聊天业务系统
*****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PEProtocol;

public class ChatSys
{
    private static ChatSys _instance = null;
    public static ChatSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ChatSys();
            }
            return _instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("ChatSys Init Done");
    }

    public void SndChat(PackMsg pack)
    {
        SndChat data = pack.msg.sndChat;
        PlayerData pd = cacheSvc.GetPlayerDataCache(pack.session);
        PshTaskProgs pshTaskProgs = null;
        //更新任务进度
        pshTaskProgs =  TaskSys.Instance.CalcTaskProgs(pd, 6);

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.PshChat,
            pshChat = new PshChat
            {
                name = pd.name,
                msg = data.msg
            }
        };
        if(pshTaskProgs != null)
        {
            msg.pshTaskProgs = pshTaskProgs;
        }

        //广播所有在线客户端
        List<ServerSession> sessionList = cacheSvc.GetOnLineServerSessions();
        foreach(ServerSession session in sessionList)
        {
            session.SendMsg(msg);
        }
    }
}

