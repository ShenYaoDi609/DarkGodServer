/****************************************************
	文件：LoginSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/05 18:57   	
	功能：登录业务系统
*****************************************************/
using PEProtocol;

public class LoginSys
{
    
    private static LoginSys _instance = null;
    public static LoginSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LoginSys();
            }
            return _instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private TimerSvc timeSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        timeSvc = TimerSvc.Instance;
        PECommon.Log("LoginSys Init Done");
    }

    public void ReqLogin( PackMsg pack)
    {
        ReqLogin data = pack.msg.reqLogin;
        ServerSession session = pack.session;
        //判断当前账号是否上线
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,
        };
        //已上线，返回错误信息
        if(cacheSvc.IsAcctOnLine(data.acct))
        {
            msg.err = (int)ErrorCode.AcctIsOnLine;
        }
        else
        {
            //未上线：
            //账号是否存在           
            PlayerData _playerData = cacheSvc.GetPlayerDate(data.acct, data.pwd);
            if(_playerData == null)
            {
                //账号存在 但密码错误
                msg.err = (int)ErrorCode.WrongPwd;
            }
            //存在 返回账号数据
            else
            {
                //计算离线增加体力
                int stamina = _playerData.stamina;
                long now = timeSvc.GetNowTime();
                long milliSecondsSpan = now - _playerData.time;
                int growStamina = (int)(milliSecondsSpan / (1000  *  60  * PECommon.StaminaAddSpace)) * PECommon.StaminaAddCount;
                //PECommon.Log(milliSecondsSpan / 60000+ " " + milliSecondsSpan.ToString() + " " + growStamina.ToString());
                if (growStamina > 0)
                {
                    int maxStamina = PECommon.GetStaminaLimitByLv(_playerData.lv);
                    if(_playerData.stamina < maxStamina)
                    {
                        _playerData.stamina += growStamina;
                        if(_playerData.stamina > maxStamina)
                        {
                            _playerData.stamina = maxStamina;
                        }
                    }                                    
                }

                if(stamina != _playerData.stamina)
                {
                    cacheSvc.UpdatePlayerData(_playerData.id, _playerData);
                }

                msg.rspLogin = new RspLogin
                {
                    //将账号数据存到回应包里
                    playerData = _playerData
                };
                //将账号数据存储进缓存层
                cacheSvc.StoreIdOnline(data.acct, session, _playerData);
            }
        }

        //回应客户端
        pack.session.SendMsg(msg);
    }

    //回应重命名请求
    public void ReqRename(PackMsg pack)
    {
        ReqRename data = pack.msg.reqRename;
        ServerSession session = pack.session;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspRename,
        };

        //判断当前名字是否存在
        //若存在，返回错误码
        if(cacheSvc.IsNameExist(data.name))
        {
            msg.err = (int)ErrorCode.NameIsExist;
        }
        //若不存在，更新缓存以及数据库，返回客户端
        else
        {
            PlayerData playerData = cacheSvc.GetPlayerDataCache(session);

             playerData.name = data.name;

            //数据库更新失败
            if(!cacheSvc.UpdatePlayerData(playerData.id,playerData))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspRename = new RspRename
                {
                    name = data.name,
                };
            }
        }
        pack.session.SendMsg(msg);
    }

    public void ClearOfflineData(ServerSession serverSession)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataCache(serverSession);
        if(playerData != null)
        {
            playerData.time = timeSvc.GetNowTime();
            if(!cacheSvc.UpdatePlayerData(playerData.id,playerData))
            {
                PECommon.Log("Update OffLine Time Error",LogType.Error);
            }
            cacheSvc.ClearOffLineData(serverSession);
        }
        
    }
}

