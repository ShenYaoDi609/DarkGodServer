using Protocal;
using PENet;

public class ServerSession : PESession<NetMsg>
{
    protected override void OnConnected()
    {
        PETool.LogMsg("Client Connect");
        SendMsg(new NetMsg { text = "Welcome to connect" });
    }

    protected override void OnReciveMsg(NetMsg msg)
    {
        PETool.LogMsg("Client Reg:" + msg.text);
        SendMsg(new NetMsg { text = "SrvRsp:" + msg.text });
    }

    protected override void OnDisConnected()
    {
        PETool.LogMsg("Client DisConnect");
    }
}


