using Protocal;
using UnityEngine;

public class ClientSession:PENet.PESession<NetMsg>
{
    protected override void OnConnected()
    {
        Debug.Log("Client Connected");

    }

    protected override void OnReciveMsg(NetMsg msg)
    {
        Debug.Log("Client Reg:" + msg.text);
    }

    protected override void OnDisConnected()
    {
        Debug.Log("Client DisConnected");
    }
}

