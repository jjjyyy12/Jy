using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jy.Utility.Node
{
    public class RoundHash
    {
        private static readonly SortedDictionary<ulong, string> _circle = new SortedDictionary<ulong, string>();
        static void Main(string[] args)
        {
            int Replicas = 100;
            AddNode("127.0.0.1:6379", Replicas);
            AddNode("127.0.0.1:6380", Replicas);
            AddNode("127.0.0.1:6381", Replicas);
            List<string> nodes = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                nodes.Add(GetTargetNode(i + "test" + (char)i));
            }
            var counts = nodes.GroupBy(n => n, n => n.Count()).ToList();
            counts.ForEach(index => Console.WriteLine(index.Key + "-" + index.Count()));
            Console.ReadLine();
        }

        public static void AddNode(string node, int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                string identifier = node.GetHashCode().ToString() + "-" + i;
                ulong hashCode = Md5Hash(identifier);
                _circle.Add(hashCode, node);
            }
        }

        public static ulong Md5Hash(string key)
        {
            using (var hash = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                var a = BitConverter.ToUInt64(data, 0);
                var b = BitConverter.ToUInt64(data, 8);
                ulong hashCode = a ^ b;
                return hashCode;
            }
        }
        public static string GetTargetNode(string key)
        {
            ulong hash = Md5Hash(key);
            ulong firstNode = ModifiedBinarySearch(_circle.Keys.ToArray(), hash);
            return _circle[firstNode];
        }

        /// <summary>
        /// 计算key的数值，得出空间归属。
        /// </summary>
        /// <param name="sortedArray"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ulong ModifiedBinarySearch(ulong[] sortedArray, ulong val)
        {
            int min = 0;
            int max = sortedArray.Length - 1;

            if (val < sortedArray[min] || val > sortedArray[max])
                return sortedArray[0];

            while (max - min > 1)
            {
                int mid = (max + min) / 2;
                if (sortedArray[mid] >= val)
                {
                    max = mid;
                }
                else
                {
                    min = mid;
                }
            }

            return sortedArray[max];
        }
    }
}
