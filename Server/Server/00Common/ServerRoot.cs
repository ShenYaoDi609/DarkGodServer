/****************************************************
	文件：ServerRoot.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/05 18:51   	
	功能：服务器初始化
*****************************************************/

public class ServerRoot
{
    private static ServerRoot _instance = null;
    public static ServerRoot Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new ServerRoot();
            }
            return _instance;
        }
    }

    public void Init()
    {
        //数据层TODO
        DBManager.Instance.Init();

        //服务层
        NetSvc.Instance.Init();
        CacheSvc.Instance.Init();
        CfgSvc.Instance.Init();
        TimerSvc.Instance.Init();

        //业务系统
        LoginSys.Instance.Init();
        AutoGuideSys.Instance.Init();
        StrongSys.Instance.Init();
        ChatSys.Instance.Init();
        PurchaseSys.Instance.Init();
        PowerSys.Instance.Init();
        TaskSys.Instance.Init();
        FuBenSys.Instance.Init();
    }

    public void Update()
    {
        NetSvc.Instance.Update();
        TimerSvc.Instance.Update();
    }

    private int sessionNum = 0;
    public int GetSessionID()
    {
        if(sessionNum == int.MaxValue)
        {
            sessionNum = 0;
        }
        return sessionNum += 1;
    }
}

