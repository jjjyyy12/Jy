using org.apache.zookeeper;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Jy.ServicesKeep
{
    public class Register : ClientBase, IRegister
    {

        public Register(string hostPort) : base(hostPort)
        {
        }

        public async Task<string> createAsync(string node, string name,string port, Watcher watcher)
        {
            ZooKeeper zk = createClient(watcher);
            //await zk.setDataAsync("/", Encoding.UTF8.GetBytes("t"), -1);
            try
            {
                if (zk.existsAsync(node).Result == null)
                {
                    await zk.createAsync($"{node}", Encoding.UTF8.GetBytes($"{name}:{port}"), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
                }//$"{node}/a"
                //need watcher else excption EPHEMERAL disconnected
                return await zk.createAsync($"{node}/a", Encoding.UTF8.GetBytes($"{name}:{port}"), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);
            }
            catch (KeeperException ke)
            {
                return await Task.FromResult($"Keeper error:{ke.Message}");
            }
            catch (Exception ex) 
            {
                return await Task.FromResult($"error:{ex.Message}");
            }
             
        }
        public string removeAsync(string node, Watcher watcher)
        {
            ZooKeeper zk = createClient(watcher);
            if (zk.existsAsync(node, null).Result != null)
            {
                zk.deleteAsync(node);
                return "ok";
            }
            else
                return "notexists";
        }

    }
}
