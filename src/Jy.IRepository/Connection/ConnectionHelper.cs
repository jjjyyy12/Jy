using Jy.Utility.Node;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IRepositories
{
    public class ConnectionHelper
    {
        public static string getConnectionFromId(string id, string spliter,string connectionKeyList, string connectionList)
        {
            List<string> keys = new List<string>(connectionKeyList.Split(new string[] { spliter }, StringSplitOptions.RemoveEmptyEntries));
            KetamaNodeLocator k = new KetamaNodeLocator(keys);
            var key = k.GetPrimary(id);

            List<string> conns = new List<string>(connectionList.Split(new string[] { spliter }, StringSplitOptions.RemoveEmptyEntries));
            int resindex = 0;
            Int32.TryParse(key, out resindex);
            return conns[resindex - 1];
        }
    }
}
