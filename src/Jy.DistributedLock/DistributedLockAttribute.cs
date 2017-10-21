using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.DistributedLock
{
    /// <summary>
    /// 分布式锁标签，构造函数为 keyname 和 超时时间
    /// </summary>
    public class DistributedLockAttribute : Attribute
    {
        private string _keyname;
        public string keyname
        {
            set { _keyname = value; }
            get { return _keyname; }
        }
        private  int _exptime;
        public int exptime
        {
            set { _exptime = value; }
            get { return _exptime; }
        }
        public DistributedLockAttribute( string keyname,int exptime)
        {
            _exptime = exptime;
            _keyname = keyname;
        }
    }

}
