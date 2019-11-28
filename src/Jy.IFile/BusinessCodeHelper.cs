using System;
using System.Collections.Generic;


namespace Jy.IFile
{
    public static class BusinessCodeHelper
    {
        private static Dictionary<string, string> businessCodesDict;
        private static List<string> businessCodesKeys;
        private static void InitBusinessCodesDict()
        {
            businessCodesKeys = new List<string>()
            {{ "test" },{ "lvp" },{ "sc" },{ "tpw" }
            ,{ "nb" },{ "mvp" },{ "btp" }
            };
            businessCodesDict = new Dictionary<string, string>() {
            { "test", "testfileserver" },{ "lvp", "lvpfileserver" },{ "sc", "fileserver" },{ "tpw", "fileserver" }
            ,{ "nb", "nbfileserver" },{ "mvp", "mvpfileserver" },{ "btp", "btpfileserver" }
            };
        }

        public static string GetBusinessCode(string host)
        {
            if (businessCodesDict == null)
            {
                InitBusinessCodesDict();
            }
            string key = "sc";
            businessCodesKeys.ForEach(x => {
                if (host.AsSpan().IndexOf(x) > -1)
                    key = x;
            });
            string businessCode = "";
            if (businessCodesDict.ContainsKey(key))
                businessCode = businessCodesDict[key];
            return businessCode;
        }

    }
}
