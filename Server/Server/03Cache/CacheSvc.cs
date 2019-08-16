/****************************************************
	文件：CacheSvc.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/08 15:59   	
	功能：数据缓存层
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
public class CacheSvc
{
    private static CacheSvc _instance = null;
    public static CacheSvc  Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CacheSvc();
            }
            return _instance;
        }
    }

    private DBManager dbManager = null;
    //存储上线账号以及其对应连接的字典
    private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
    //缓存上线账号数据
    private Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();

    public void Init()
    {
        dbManager = DBManager.Instance;
        PECommon.Log("CacheSvc Init Done.");
    }

    /// <summary>
    /// 判断账号是否上线
    /// </summary>
    public bool IsAcctOnLine(string acct)
    {
        return onLineAcctDic.ContainsKey(acct);
    }
    
    /// <summary>
    /// 清理下线数据
    /// </summary>
    public void ClearOffLineData(ServerSession serverSession)
    {
        foreach(var temp in onLineAcctDic)
        {
            if(temp.Value.sessionID == serverSession.sessionID)
            {
                if(serverSession != null && onLineSessionDic.ContainsKey(serverSession))
                {                    
                    onLineSessionDic.Remove(serverSession);
                }
                onLineAcctDic.Remove(temp.Key);
                break;
            }
        }
    }

    /// <summary>
    /// 根据账号密码获取用户数据 密码错误返回null 账号不存在则默认创建新账号
    /// </summary>
    public PlayerData GetPlayerDate(string acct,string pwd)
    {
        //从数据库中查找数据
        return dbManager.QueryPlayerData(acct,pwd);
    }

    /// <summary>
    /// 从缓存中获取会话对应的玩家数据
    /// </summary>
    public PlayerData GetPlayerDataCache(ServerSession session)
    {
        PlayerData playerData = null;
        if(onLineSessionDic.ContainsKey(session))
        {
            playerData = onLineSessionDic[session];
        }
        return playerData;
    }

    /// <summary>
    /// 缓存上线账号信息
    /// </summary>
    public void StoreIdOnline(string acct,ServerSession session,PlayerData playerData)
    {
        onLineAcctDic.Add(acct, session);
        onLineSessionDic.Add(session, playerData);
    }
    
    /// <summary>
    /// 当前名字 是否已经存在
    /// </summary>
    public bool IsNameExist(string name)
    {
        return dbManager.QueryNameData(name);
    }

    public bool UpdatePlayerData(int id,PlayerData playerData)
    {        
        return dbManager.UpdateAcctData(id,playerData);
    }

    public List<ServerSession> GetOnLineServerSessions()
    {
        List<ServerSession> sessionList = new List<ServerSession>();
        foreach(ServerSession session in onLineSessionDic.Keys)
        {
            if(!sessionList.Contains(session))
            {
                sessionList.Add(session);
            }
        }
        return sessionList;
    }

    public Dictionary<ServerSession,PlayerData> GetOnlineSessionDic()
    {
        return onLineSessionDic;
    }

    public ServerSession GetOnLineServerSession(int id)
    {
        ServerSession session = null;
        foreach(var item in onLineSessionDic)
        {
            if(item.Value.id ==  id)
            {
                session = item.Key;
                break;
            }
        }
        return session;
    }
}

