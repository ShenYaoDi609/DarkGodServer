/****************************************************
	文件：TimerSvc.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/20 9:19   	
	功能：定时系统（服务器端）
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TimerSvc
{
    private static TimerSvc _instance = null;
    public static TimerSvc Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TimerSvc();
            }
            return _instance;
        }
    }

    private PETimer peTimer;
    private Queue<TaskPack> tpQue = new Queue<TaskPack>();
    private static readonly string tpQueLock = "tpQueLock";

    public void Init()
    {
        peTimer = new PETimer(100);
        tpQue.Clear();

        peTimer.SetLog((string info) =>
        {
            PECommon.Log(info);
        });

        peTimer.SetHandle((Action<int> callback, int timeID) =>
        {
            if(callback != null)
            {
                lock(tpQueLock)
                {
                    tpQue.Enqueue(new TaskPack(timeID, callback));
                }
            }
        });

        PECommon.Log("TimerSvc Init Done....");
    }

    public void Update()
    {
        //peTimer.Update();
        while(tpQue.Count > 0)
        {
            TaskPack tp = null;
            lock(tpQueLock)
            {
                tp = tpQue.Dequeue();
            }
            if(tp != null)
            {
                tp.callback(tp.timeID);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int loopTime = 0)
    {
        return peTimer.AddTimeTask(callback, delay, timeUnit, loopTime);
    }

    private class TaskPack
    {
        public int timeID;
        public Action<int> callback;
        public TaskPack(int timeID,Action<int> callback)
        {
            this.timeID = timeID;
            this.callback = callback;
        }
    }

    public long GetNowTime()
    {
        return (long)peTimer.GetMillisecondsTime();
    }
}


