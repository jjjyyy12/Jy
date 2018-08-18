using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Cache
{
    /// <summary>
    /// 哈希节点对象
    /// </summary>
    public class ConsistentHashNode : CacheEndpoint
    {
        /// <summary>
        /// 缓存目标类型
        /// </summary>
        public CacheTargetType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 主机
        /// </summary>
        public new string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public new string Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public string Db
        {
            get; set;
        }

        private string _maxSize = "50";
        public string MaxSize
        {
            get
            {
                return _maxSize;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _maxSize = value;
                }
            }
        }

        private string _minSize = "1";
        public string MinSize
        {
            get
            {
                return _minSize;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _minSize = value;
                }
            }
        }

        public override string ToString()
        {
            return string.Concat(new string[] { Host, ":", Port.ToString() });
        }
    }
}
