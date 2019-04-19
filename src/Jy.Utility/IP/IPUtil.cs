using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jy.Utility.IP
{
    public class IPUtil
    {
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名  
                IPHostEntry IpEntry = Dns.GetHostEntryAsync(HostName).Result;
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址  
                    //AddressFamily.InterNetwork表示此IP为IPv4,  
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型  
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        
        public static string GetLocalIPDebug()
        {
            return "localhost";
        }
    }
}
