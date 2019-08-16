using PEProtocol;
using System.Collections.Generic;
using System.Xml;


public class CfgSvc
{
    private static CfgSvc _instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CfgSvc();
            }
            return _instance;
        }
    }

    public void Init()
    {
        InitAutoGuideCfgs();
        InitStrongCfg();
        InitTaskRewardCfgs();
        InitMapCfgs();
        PECommon.Log("CfgSvc Init Done...");
    }


    #region 地图信息
    public Dictionary<int, MapCfg> mapCfgDits = new Dictionary<int, MapCfg>();
    public void InitMapCfgs()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(@"D:\Unity Project WorkSpace\DarkGod\Assets\Resources\ResCfgs\map.xml");

        //取得根结点
        XmlNode root = xmlDoc.SelectSingleNode("root");
        //取得根节点下所有子节点
        XmlNodeList nodeList = root.ChildNodes;

        foreach (XmlNode mapNode in nodeList)
        {
            int ID = int.Parse(mapNode.Attributes["ID"].Value);
            MapCfg mapCfg = new MapCfg
            {
                ID = ID
            };

            XmlNodeList fieldNodeList = mapNode.ChildNodes;
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "power":
                        mapCfg.costStamina = int.Parse(fieldNode.InnerText);
                        break;
                    case "exp":
                        mapCfg.exp = int.Parse(fieldNode.InnerText);
                        break;
                    case "coin":
                        mapCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                    case "crystal":
                        mapCfg.crystal = int.Parse(fieldNode.InnerText);
                        break;
                    default:
                        break;
                }
            }
            if (!mapCfgDits.ContainsKey(ID))
            {
                mapCfgDits.Add(ID, mapCfg);
            }
        }
    }
    public MapCfg GetMapCfg(int id)
    {
        MapCfg mapCfg;
        if (mapCfgDits.TryGetValue(id, out mapCfg))
        {
            return mapCfg;
        }
        return null;
    }
    #endregion

    #region 自动寻路配置

    public Dictionary<int, AutoGuideCfg> autoGuideCfgDicts = new Dictionary<int, AutoGuideCfg>();
    public void InitAutoGuideCfgs()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(@"D:\Unity Project WorkSpace\DarkGod\Assets\Resources\ResCfgs\guide.xml");

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;

        foreach (XmlNode taskNode in nodeList)
        {
            int _taskID = int.Parse(taskNode.Attributes["ID"].Value);
            AutoGuideCfg autoGuideCfg = new AutoGuideCfg
            {
                ID = _taskID
            };
            XmlNodeList fieldNodeList = taskNode.ChildNodes;
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "coin":
                        autoGuideCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                    case "exp":
                        autoGuideCfg.exp = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            if (!autoGuideCfgDicts.ContainsKey(_taskID))
            {
                autoGuideCfgDicts.Add(_taskID, autoGuideCfg);
            }
        }
    }

    public AutoGuideCfg GetAutoGuideCfg(int id)
    {
        AutoGuideCfg autoGuideCfg = null;
        if (autoGuideCfgDicts.TryGetValue(id, out autoGuideCfg))
        {
            return autoGuideCfg;
        }
        return null;
    }
    #endregion

    #region 强化配置
    public Dictionary<int, Dictionary<int, StrongCfg>> strongDict = new Dictionary<int, Dictionary<int, StrongCfg>>();
    public void InitStrongCfg()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(@"D:\Unity Project WorkSpace\DarkGod\Assets\Resources\ResCfgs\strong.xml");

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;
        foreach (XmlNode strongNode in nodeList)
        {
            int id = int.Parse(strongNode.Attributes["ID"].Value);
            StrongCfg strongCfg = new StrongCfg
            {
                ID = id
            };
            XmlNodeList fieldNodeList = strongNode.ChildNodes;
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "pos":
                        strongCfg.pos = int.Parse(fieldNode.InnerText);
                        break;
                    case "starlv":
                        strongCfg.starlv = int.Parse(fieldNode.InnerText);
                        break;
                    case "addhp":
                        strongCfg.addhp = int.Parse(fieldNode.InnerText);
                        break;
                    case "addhurt":
                        strongCfg.addhurt = int.Parse(fieldNode.InnerText);
                        break;
                    case "adddef":
                        strongCfg.adddef = int.Parse(fieldNode.InnerText);
                        break;
                    case "minlv":
                        strongCfg.minlv = int.Parse(fieldNode.InnerText);
                        break;
                    case "coin":
                        strongCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                    case "crystal":
                        strongCfg.crystal = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            Dictionary<int, StrongCfg> dic = null;
            //如果有对应的键pos，说明这类装备已经有部分升级配置信息被录入,直接加入
            if (strongDict.TryGetValue(strongCfg.pos, out dic))
            {
                dic.Add(strongCfg.starlv, strongCfg);
            }
            else
            {
                dic = new Dictionary<int, StrongCfg>();
                dic.Add(strongCfg.starlv, strongCfg);
                strongDict.Add(strongCfg.pos, dic);
            }

        }
    }

    public StrongCfg GetStrongCfg(int pos, int starlv)
    {
        StrongCfg strongCfg = null;
        Dictionary<int, StrongCfg> dic = null;
        if (strongDict.TryGetValue(pos, out dic))
        {
            if (dic.ContainsKey(starlv))
            {
                strongCfg = dic[starlv];
            }
        }
        return strongCfg;
    }

    #endregion

    #region 任务配置
    public Dictionary<int, TaskRewardCfg> taskRewardCfgDicts = new Dictionary<int, TaskRewardCfg>();
    public void InitTaskRewardCfgs()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(@"D:\Unity Project WorkSpace\DarkGod\Assets\Resources\ResCfgs\taskreward.xml");

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;

        foreach (XmlNode taskNode in nodeList)
        {
            int _taskID = int.Parse(taskNode.Attributes["ID"].Value);
            TaskRewardCfg taskrewardCfg = new TaskRewardCfg
            {
                ID = _taskID
            };
            XmlNodeList fieldNodeList = taskNode.ChildNodes;
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "count":
                        taskrewardCfg.count = int.Parse(fieldNode.InnerText);
                        break;
                    case "coin":
                        taskrewardCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                    case "exp":
                        taskrewardCfg.exp = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            if (!taskRewardCfgDicts.ContainsKey(_taskID))
            {
                taskRewardCfgDicts.Add(_taskID, taskrewardCfg);
            }
        }
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg taskRewardCfg = null;
        if (taskRewardCfgDicts.TryGetValue(id, out taskRewardCfg))
        {
            return taskRewardCfg;
        }
        return null;
    }
    #endregion

}

public class MapCfg : BaseData<MapCfg>
{
    public int costStamina;
    public int exp;
    public int coin;
    public int crystal;
}

public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}

public class AutoGuideCfg : BaseData<AutoGuideCfg>
{
    public int coin;
    public int exp;
}

public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int count;
    public int exp;
    public int coin;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int progress;
    public bool taked;
}

public class BaseData<T>
{
    public int ID;
}

