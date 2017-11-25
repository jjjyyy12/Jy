using Jy.Utility.Node;
using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.ServicesKeep
{
    public class CallServer : ClientBase, ICallServer
    {
        public CallServer(string hostPort) : base(hostPort)
        {
        }

        public string GetServer(string node, Watcher watcher)
        {
            ZooKeeper zk = createClientForCall(watcher);
            if (zk.existsAsync(node, null).Result != null)
            {
                string res = Encoding.UTF8.GetString(zk.getDataAsync(node).Result.Data);
                return res;
            }
            else
                return "";
        }

        public string getChild(string node,string id, Watcher watcher)
        {
            ZooKeeper zk = createClientForCall(watcher);
            var children = GetChild(ref zk,node, watcher);
            
            List<string> childList = children?.Children;
            if (childList?.Count>0)
            {
                int i = childList.Count;
                KetamaNodeLocator k = new KetamaNodeLocator(childList);
                var key = k.GetPrimary(id);
                return GetServer(zk, $"{node}/{key}");
            }
            else
                return "";
        }
        //得到node下所有注册的服务节点列表，连接异常的话重试4回
        private ChildrenResult GetChild(ref ZooKeeper zk,string node, Watcher watcher ,int runcnt=0)
        {
            if(zk == null)
              zk = createClientForCall(watcher);
            ChildrenResult children;
            try
            {
                var state = zk.getState();
                if(state!=ZooKeeper.States.CONNECTED)
                    zk = createClientForCall(watcher);
                children = zk.getChildrenAsync(node,true).Result;
            }
            catch (AggregateException ex)
            {
                if (runcnt <= 4) //4次机会重发,每次等待2*失败次数的时间
                {
                    runcnt++;
                    Thread.Sleep(1500 * runcnt);
                    children = GetChild(ref zk,node, watcher, runcnt);
                }
                else
                {
                    children = null;
                }
            }
            return children;
        }
        private string GetServer(ZooKeeper zk,string node)
        {
            try
            {
                if (zk.existsAsync(node, true).Result != null)
                {
                    return Encoding.UTF8.GetString(zk.getDataAsync(node).Result.Data);
                }
                else
                    return "";
            }catch(Exception ee)
            {
                return "";
            }
        }
        

    }
}
