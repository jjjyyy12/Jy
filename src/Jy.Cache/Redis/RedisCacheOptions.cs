using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Cache
{
    public class RedisCacheOptions
    {
        public string Configuration { get; set; }
        public string InstanceName { get; set; }
        public string Pwd { get; set; }
        public int ConnectTimeout { get; set; }
        public TimeSpan expTime { get; set; } = new TimeSpan(0, 10, 0);

    }
}
