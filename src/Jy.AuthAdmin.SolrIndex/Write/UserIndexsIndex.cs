using AutoMapper;
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
    ///  
    /// </summary>
    public class UserIndexsIndex : IndexBase<UserIndexs, AuthAdminSolrServer<UserIndexs>>, IUserIndexsIndex
    {

        public UserIndexsIndex(AuthAdminSolrServer<UserIndexs> solrServer) : base(solrServer, solrServer)
        {
        }

        public UserIndexs EditUserIndex(User user, bool autoSave = true)
        {
            var indexs = Mapper.Map<UserIndexs>(user);
            return Update(indexs);
        }

        public UserIndexs InsertUserIndex(User user, bool autoSave = true)
        {
            var indexs = Mapper.Map<UserIndexs>(user);
            return Insert(indexs);
        }
    }
}
