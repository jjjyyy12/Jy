using System;
using System.Collections.Generic;
using System.Linq;

namespace Jy.Utility.Paged
{
    public class PagedHelper
    {
        public PagedHelper() { }
        /// <summary>
        /// 分页帮助类
        /// </summary>
        /// <typeparam name="T">已经取得列表中的对象类型</typeparam>
        /// <param name="rlist">已经取得列表</param>
        /// <param name="startPage">起始页1</param>
        /// <param name="pageSize">每页显示多少</param>
        /// <param name="rowCount">总行数</param>
        /// <param name="loopRlist">分页完后可能会取详细对象的委托</param>
        /// <returns></returns>
        public List<T> Paged<T>(List<T> rlist, int startPage, int pageSize, out int rowCount,Func<List<T>, List<T>> loopRlist = null)
        {
            rowCount = rlist.Count;
            if (rowCount == 0) return null;
            List<T> flist = rlist.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
            if (loopRlist != null)
                loopRlist(flist);
            return flist;
        }
    }
}
