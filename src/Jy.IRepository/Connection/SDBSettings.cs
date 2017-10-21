

namespace Jy.IRepositories
{
    public class SDBSettings
    {
        public DBType dbType { set; get; }
        
        public string connectionKeyList { set; get; }
        public string connectionList { set; get; }
        public string connectionReadList { set; get; }
        public string defaultConnectionString { set; get; }
        public string defaultReadConnectionString { set; get; }
        


    }
}
