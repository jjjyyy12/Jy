using Jy.Domain.Entities;
using Jy.Domain.IIndex;
using Jy.SolrIndex;
using Microsoft.Extensions.Options;
using SolrNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jy.AuthAdmin.SolrIndex
{
    /// <summary>
    /// 用户管理读仓储实现
    /// </summary>
    public class UserIndexsRead : IndexReadBase<UserIndexs, ISolrOperations<UserIndexs>>, IUserIndexsIndexRead
    {

        public UserIndexsRead(ISolrOperations<UserIndexs> dbcontext) : base(dbcontext)
        {
        }

        public UserIndexs CheckUserIndex(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public List<UserIndexs> GetUserIndexList(ICollection<KeyValuePair<string, string>> wheres)
        {
            throw new NotImplementedException();
        }
    }
}
