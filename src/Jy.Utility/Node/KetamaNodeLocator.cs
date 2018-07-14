using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Jy.Utility.Node
{
    //http://www.cnblogs.com/daizhj/archive/2010/08/24/1807324.html
    public class KetamaNodeLocator
    {
        //原文中的JAVA类TreeMap实现了Comparator方法，这里我图省事，直接用了net下的SortedList，其中Comparer接口方法）
        private SortedList<long, string> ketamaNodes = new SortedList<long, string>();
        private HashAlgorithmMD5 hashAlg;
        private int numReps = 160;

        //此处参数与JAVA版中有区别，因为使用的静态方法，所以不再传递HashAlgorithmMD5 alg参数
        public KetamaNodeLocator(List<string> nodes /*, int nodeCopies*/)
        {
            ketamaNodes = new SortedList<long, string>();

            //numReps = nodeCopies;
            //对所有节点，生成nCopies个虚拟结点
            foreach (string node in nodes)
            {
                //每四个虚拟结点为一组
                for (int i = 0; i < numReps / 4; i++)
                {
                    //getKeyForNode方法为这组虚拟结点得到惟一名称 
                    byte[] digest = HashAlgorithmMD5.computeMd5(node + i);
                    /** Md5是一个16字节长度的数组，将16字节的数组每四个字节一组，分别对应一个虚拟结点，这就是为什么上面把虚拟结点四个划分一组的原因*/
                    for (int h = 0; h < 4; h++)
                    {
                        long m = HashAlgorithmMD5.hash(digest, h);
                        ketamaNodes[m] = node;
                    }
                }
            }
        }

        public string GetPrimary(string k)
        {
            byte[] digest = HashAlgorithmMD5.computeMd5(k);
            string rv = GetNodeForKey(HashAlgorithmMD5.hash(digest, 0));
            return rv;
        }

        /*
         static void Main(string[] args)
        {
            //假设的server
            List<string> nodes = new List<string>() { "0001","0002" };
            KetamaNodeLocator k = new KetamaNodeLocator(nodes);
            string str = "";
            for (int i = 0; i < 10; i++)
            {
                string Key="user_" + i;
                str += string.Format("Key:{0}分配到的Server为：{1}\n\n", Key, k.GetPrimary(Key));
            }
            
            Console.WriteLine(str);
           
            Console.ReadLine();
             
        }
             */
        string GetNodeForKey(long hash)
        {
            string rv;
            long key = hash;
            //如果找到这个节点，直接取节点，返回   
            if (!ketamaNodes.ContainsKey(key))
            {
                //得到大于当前key的那个子Map，然后从中取出第一个key，就是大于且离它最近的那个key 说明详见: http://www.javaeye.com/topic/684087
                var tailMap = from coll in ketamaNodes
                              where coll.Key > hash
                              select new { coll.Key };
                if (tailMap == null || tailMap.Count() == 0)
                    key = ketamaNodes.FirstOrDefault().Key;
                else
                    key = tailMap.FirstOrDefault().Key;
            }
            rv = ketamaNodes[key];
            return rv;
        }

        string GetNodeForKey2(long hash)
        {
            string rv;
            long key = hash;
            int pos = 0;
            if (!ketamaNodes.ContainsKey(key))
            {
                int low, high, mid;
                low = 1;
                high = ketamaNodes.Count - 1;
                while (low <= high)
                {
                    mid = (low + high) / 2;
                    if (key < ketamaNodes.Keys[mid])
                    {
                        high = mid - 1;
                        pos = high;
                    }
                    else if (key > ketamaNodes.Keys[mid])
                        low = mid + 1;

                }
            }

            rv = ketamaNodes.Values[pos + 1].ToString();
            return rv;
        }
    }

    public class HashAlgorithmMD5
    {
        public static long hash(byte[] digest, int nTime)
        {
            long rv = ((long)(digest[3 + nTime * 4] & 0xFF) << 24)
                    | ((long)(digest[2 + nTime * 4] & 0xFF) << 16)
                    | ((long)(digest[1 + nTime * 4] & 0xFF) << 8)
                    | ((long)digest[0 + nTime * 4] & 0xFF);

            return rv & 0xffffffffL; /* Truncate to 32-bits */
        }

        /**
         * Get the md5 of the given key.
         */
        public static byte[] computeMd5(string k)
        {
            MD5 md5 = MD5.Create();

            byte[] keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(k));
            md5.Dispose();
            //md5.update(keyBytes);
            //return md5.digest();
            return keyBytes;
        }
    }
}
