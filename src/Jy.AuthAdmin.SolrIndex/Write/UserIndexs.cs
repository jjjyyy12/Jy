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
    ///  
    /// </summary>
    public class UserIndexsIndex : IndexBase<UserIndexs, ISolrOperations<UserIndexs>>, IUserIndexsIndex
    {

        public UserIndexsIndex(ISolrOperations<UserIndexs> dbcontext) : base(dbcontext, dbcontext)
        {
        }

        public UserIndexs EditUserIndex(User user, bool autoSave = true)
        {
            throw new NotImplementedException();
        }

        public UserIndexs InsertUserIndex(User user, bool autoSave = true)
        {
            throw new NotImplementedException();
        }
    }
}
