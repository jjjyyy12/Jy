using Jy.DapperBase;
using Jy.DapperBase.Repositories;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories; 
using Jy.IRepositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; 

namespace Jy.Dapper.Repositories
{
    /// <summary>
    /// 用户管理读仓储实现
    /// </summary>
    public class UserRepositoryRead : DapperRepositoryReadBase<User>, IUserRepositoryRead
    {

        protected readonly TransactedConnection dbContext;
        public UserRepositoryRead(IRepositoryReadContext context) : base(context)
        {
            dbContext = _dbContext;
        }

        /// <summary>
        /// 从总库中索引表中取出userid
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>存在返回用户实体，否则返回NULL</returns>
        public UserIndex CheckUserIndex(string userName, string password)
        {
            return base.GetAllList<UserIndex>((u) => { u => u.UserName == userName && u.Password == password}).FirstOrDefault();
        }
        public List<UserIndex> GetUserIndexList(Expression<Func<UserIndex, bool>> predicate)
        {
            return dbContext.UserIndexs.Where(predicate).ToList();
        }
        /// <summary>
        /// 检查用户是存在
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>存在返回用户实体，否则返回NULL</returns>
        public User CheckUser(string userName, string password)
        {
            var query = from u in dbContext.Users
                        where u.UserName == userName && u.Password == password
                        join x in dbContext.Departments on u.DepartmentId equals x.Id into temp
                        from x in temp.DefaultIfEmpty()
                        select new User()
            {
                Id = u.Id
                ,DepartmentId = u.DepartmentId
                ,UserName = u.UserName
                ,Name = u.Name
                ,EMail = u.EMail
                ,MobileNumber = u.MobileNumber
                ,Department = x
            };
            var dd = query.Cast<User>().FirstOrDefault();
            var rr = from ur in dbContext.UserRoles where ur.UserId == dd.Id select ur;
                dd.UserRoles = rr.ToList();
            return dd;

            //return _dbContext.Set<User>().FirstOrDefault(it => it.UserName == userName && it.Password == password);
        }
        public User GetUserInfo(Guid userId)
        {
            var query = from u in dbContext.Users
                        where u.Id == userId  
                        join x in dbContext.Users on u.CreateUserId equals x.Id into temp
                        from x in temp.DefaultIfEmpty()
                        select new User()
                        {
                            Id = u.Id,
                            DepartmentId = u.DepartmentId,
                            UserName = u.UserName,
                            Name = u.Name,
                            EMail = u.EMail,
                            MobileNumber = u.MobileNumber,
                            Remarks = u.Remarks,
                            LastLoginTime = u.LastLoginTime,
                            LoginTimes = u.LoginTimes,
                            CreateTime = u.CreateTime,
                            IsDeleted = u.IsDeleted,
                            CreateUserId =u.CreateUserId,
                            CreateUser = x
                        };
            var dd = query.Cast<User>().FirstOrDefault();
            return dd;
        }
        public List<UserRole> GetUserRoles(Guid id)
        {
            return dbContext.Set<UserRole>().Where(it => it.UserId == id).ToList();
        }
     
    }
}
