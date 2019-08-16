using MySql.Data;
using MySql.Data.MySqlClient;
using PEProtocol;
using System;

public class DBManager
{
    private static DBManager _instance = null;
    public static DBManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DBManager();
            }
            return _instance;
        }
    }

    private MySqlConnection conn = null;

    public void Init()
    {
        //连接数据库
        conn = new MySqlConnection("server=localhost;User Id = root;password=;Database=darkgod;Charset=utf8");
        conn.Open();
        PECommon.Log("DBManager Init Done...");
    }

    
    public PlayerData QueryPlayerData(string acct,string pwd)
    {

        PlayerData playerData = null;
        MySqlDataReader reader = null;
        bool isNewAcct = true;

        try
        {
            //从数据库中查询出账号所对应的PlayerData
            MySqlCommand cmd = new MySqlCommand("select * from account where acct = @acct", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                isNewAcct = false;
                string _pwd = reader.GetString("pass");
                //密码正确 返回玩家数据
                if(_pwd.Equals(pwd))
                {
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        lv = reader.GetInt32("lv"),
                        exp = reader.GetInt32("exp"),
                        stamina = reader.GetInt32("stamina"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond"),
                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),
                        taskid = reader.GetInt32("taskid"),
                        crystal = reader.GetInt32("crystal"),
                        time = reader.GetInt64("time"),
                        fuben = reader.GetInt32("fuben"),
                        energy =  reader.GetInt32("energy"),
                    };

                    string[] strongStrArr = reader.GetString("strong").Split('#');
                    int[] strongArr = new int[6];
                    for(int i = 0; i < strongStrArr.Length; i++)
                    {
                        if(strongStrArr[i] == "")
                        {
                            continue;
                        }
                        strongArr[i] = int.Parse(strongStrArr[i]);
                    }
                    playerData.strongArr = strongArr;

                    #region Task Data
                    //1|0|0#1|0|0#1|0|0
                    string[] taskArr = reader.GetString("task").Split('#');
                    playerData.taskArr = new string[6];
                    for(int i = 0; i < taskArr.Length; i++)
                    {
                        if (taskArr[i] == "")
                        {
                            continue;
                        }
                        else if (taskArr[i].Length >= 5)
                        {
                            playerData.taskArr[i] = taskArr[i];
                        }
                        else
                        {
                            throw new Exception("DataErroe");
                        }
                    }
                    #endregion
                }

            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query PlayerDate By Acct&Pwd Error:" + e, LogType.Error);
        }
        finally
        {
            if(reader != null)
            {
                //执行ExecuteNonQuery前要关闭ExecuteReader
                reader.Close();
            }
            //如果是新账号就自动创建
            if(isNewAcct == true)
            {
                playerData = new PlayerData
                {
                    id = -1,
                    name = "",
                    lv = 1,
                    exp = 0,
                    stamina = 150,
                    coin = 500,
                    diamond = 100,
                    crystal = 500,
                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,
                    taskid = 1001,
                    strongArr = new int[6] { 0, 0, 0, 0, 0, 0 },
                    time = TimerSvc.Instance.GetNowTime(),
                    taskArr = new string[6],
                    fuben = 10001,
                    energy = 100,
                    //TODO
                };
                //初始化任务奖励数据
                for(int i = 0; i < playerData.taskArr.Length; i++)
                {
                    playerData.taskArr[i] = (i + 1) + "|0|0";
                }

                //向数据库插入数据
                playerData.id = InsertNewAcctData(acct, pwd, playerData);
            }
        }

        return playerData;
    }

    private int InsertNewAcctData(string acct,string pwd,PlayerData playerData)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand("insert into account set acct = @acct, pass = @pass, name = @name, lv = @lv, exp = @exp, stamina = @stamina,coin = @coin, diamond = @diamond, " +
                "crystal = @crystal,hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce,critical = @critical,taskid = @taskid,strong = @strong,time = @time,task = @task,fuben = @fuben,energy = @energy", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pwd);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("lv", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("stamina", playerData.stamina);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);
            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce",playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("taskid", playerData.taskid);
            cmd.Parameters.AddWithValue("time", playerData.time);
            cmd.Parameters.AddWithValue("fuben", playerData.fuben);
            cmd.Parameters.AddWithValue("energy", playerData.energy);

            int[] strongArr = playerData.strongArr;
            string strongStrArr = "";
            for(int i = 0; i < strongArr.Length; i++)
            {
                strongStrArr += strongArr[i].ToString();
                strongStrArr += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongStrArr);

            string[] taskArr = playerData.taskArr;
            string taskStrArr = "";
            for(int i = 0; i < taskArr.Length; i++)
            {
                taskStrArr += taskArr[i];
                taskStrArr += "#";
            }
            cmd.Parameters.AddWithValue("task", taskStrArr);

            cmd.ExecuteNonQuery();

            id = (int)cmd.LastInsertedId;
        }
        catch(Exception e)
        {
            PECommon.Log("Insert New Accout Error:" + e, LogType.Error);
        }
 
        PECommon.Log(id.ToString());
        return id;
    }

    /// <summary>
    /// 更新数据库的某一条记录
    /// </summary>
    public bool UpdateAcctData(int id,PlayerData playerData)
    {
        bool updateSucceed = false;
        try
        {
            MySqlCommand cmd = new MySqlCommand("update account set name = @name, lv = @lv,exp = @exp, stamina = @stamina,coin = @coin,diamond = @diamond," +
                "crystal = @crystal, hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce,critical = @critical,taskid = @taskid," +
                "strong = @strong,time = @time,task = @task,fuben = @fuben,energy = @energy where id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("lv", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("stamina", playerData.stamina);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);
             cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce",playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("taskid", playerData.taskid);
            cmd.Parameters.AddWithValue("time", playerData.time);
            cmd.Parameters.AddWithValue("fuben", playerData.fuben);
            cmd.Parameters.AddWithValue("energy", playerData.energy);

            int[] strongArr = playerData.strongArr;
            string strongStrArr = "";
            for (int i = 0; i < strongArr.Length; i++)
            {
                strongStrArr += strongArr[i].ToString();
                strongStrArr += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongStrArr);

            string[] taskArr = playerData.taskArr;
            string taskStrArr = "";
            for (int i = 0; i < taskArr.Length; i++)
            {
                taskStrArr += taskArr[i];
                taskStrArr += "#";
            }
            cmd.Parameters.AddWithValue("task", taskStrArr);

            cmd.ExecuteNonQuery();
            updateSucceed = true;
        }
        catch(Exception e)
        {
            PECommon.Log("Update Accout Data Error:" + e, LogType.Error);
        }
        return updateSucceed;

    }

    /// <summary>
    /// 搜索玩家名
    /// </summary>
    public bool QueryNameData(string name)
    {
        MySqlDataReader reader = null;
        bool exist = false;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where name = @name", conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            //名字已存在 返回true
            if(reader.Read())
            {
                exist =  true;
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query NameData By Name Error:" + e, LogType.Error);
        }
        finally
        {
            if(reader != null)
            {
                reader.Close();
            }
        }
        return exist;
    }
    
}

