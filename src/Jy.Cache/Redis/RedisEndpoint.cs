using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Cache
{
    /// <summary>
    /// redis 终端
    /// </summary>
    public class RedisEndpoint : CacheEndpoint
    {
        /// <summary>
        /// 主机
        /// </summary>
        public new string Host
        {
            get; set;
        }

        /// <summary>
        /// 端口
        /// </summary>
        public new int Port
        {
            get; set;
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get; set;
        }

        /// <summary>
        /// 数据库
        /// </summary>
        public int DbIndex
        {
            get; set;
        }

        public int MaxSize
        {
            get; set;
        }

        public int MinSize
        {
            get;
            set;
        }


        public override string ToString()
        {
            return string.Concat(new string[] { Host, ":", Port.ToString(), "::", DbIndex.ToString() });
        }

    }
}
