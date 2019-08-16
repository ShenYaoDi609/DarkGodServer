/****************************************************
	文件：ServerSession.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/05 19:21   	
	功能：网络会话连接
*****************************************************/
using PENet;
using PEProtocol;

public class ServerSession:PESession<GameMsg>
{
    public int sessionID = 0;
    protected override void OnConnected()
    {
        sessionID = ServerRoot.Instance.GetSessionID();
        PECommon.Log("SessionID:" + sessionID + " Client Connect");

    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("SessionID:" + sessionID + " Client Req:" + ((CMD)(msg.cmd)).ToString());
        NetSvc.Instance.AddMsgQue(this, msg);
    }

    protected override void OnDisConnected()
    {
        LoginSys.Instance.ClearOfflineData(this);
        PECommon.Log("SessionID:" + sessionID + " Client Disconnect");
    }
}

