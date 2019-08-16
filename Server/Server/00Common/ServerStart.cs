/****************************************************
	文件：Program.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/05 18:50   	
	功能：服务器入口
*****************************************************/
using System.Threading;

class ServerStart
{
    static void Main(string[] args)
    {
        ServerRoot.Instance.Init();

        while (true)
        {
            ServerRoot.Instance.Update();
            Thread.Sleep(20);            
        }
    }
}