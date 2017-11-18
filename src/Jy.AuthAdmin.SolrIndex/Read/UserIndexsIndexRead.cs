using Jy.Domain.Entities;
using Jy.Domain.IIndex;
using Jy.SolrIndex;
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
    public class UserIndexsIndexRead : IndexReadBase<UserIndexs, AuthAdminSolrServer<UserIndexs>>, IUserIndexsIndexRead
    {

        public UserIndexsIndexRead(AuthAdminSolrServer<UserIndexs> dbcontext) : base(dbcontext)
        {
        }

        public UserIndexs CheckUserIndex(string userName, string password)
        {
            return FirstOrDefault(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>( "username", userName ), new KeyValuePair<string, string> ("password", password ) });
        }

        public List<UserIndexs> GetUserIndexList(ICollection<KeyValuePair<string, string>> wheres)
        {
            return GetAllList(wheres);
        }
    }
}
