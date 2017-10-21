﻿using Jy.Domain.IRepositories;
using Jy.IRepositories;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Jy.EntityFrameworkCore.Repositories
{
    public class RepositoryFactory: IRepositoryFactory
    {
        private readonly IOptionsSnapshot<SDBSettings> _SDBSettings;
        private readonly CreateRepository _createRepository = new CreateRepository();
        public RepositoryFactory(IOptionsSnapshot<SDBSettings> SDBSettings)
        {
            _SDBSettings = SDBSettings;
            if (_createRepository?.ConfigDictionary.Count == 0)
            {
                _createRepository.AddConfig<IUserRepository, UserRepository>();
                _createRepository.AddConfig<IRoleRepository, RoleRepository>();
                _createRepository.AddConfig<IMenuRepository, MenuRepository>();
                _createRepository.AddConfig<IDepartmentRepository, DepartmentRepository>();
            }
        }
        private  JyDbContext getContext(string Id)
        {
            return BuildDBContext.CreateJyDBContextFromId(Id, _SDBSettings.Value.connectionKeyList, _SDBSettings.Value.connectionList,_SDBSettings.Value.defaultConnectionString,_SDBSettings.Value.dbType);
        }
        public HashSet<string> GetConnectionStrings()
        {
           return BuildDBContext.GetConnectionStrings(_SDBSettings.Value.connectionList,_SDBSettings.Value.defaultConnectionString, _SDBSettings.Value.dbType);
        }

        public TH CreateRepository<T, TH>(string Id)
            where T : Entity
            where TH : IRepository<T>
        {
            var contextObj = getContext(Id);
            return _createRepository.Get<TH>(new object[] { contextObj });
        }
        public TH CreateDefaultRepository<T, TH>()
           where TH : IRepository<T>
           where T : Entity
        {
            var contextObj = BuildDBContext.CreateJyDBContext(_SDBSettings.Value.defaultConnectionString, _SDBSettings.Value.dbType);
            return _createRepository.Get<TH>(new object[] { contextObj });
        }

        //得到所有分库的Repository
        public List<TH> CreateAllRepository<T, TH>()
            where T : Entity
            where TH : IRepository<T>
        {
            var contextList = BuildDBContext.CreateAllJyDBContext(_SDBSettings.Value.connectionList, _SDBSettings.Value.defaultConnectionString,_SDBSettings.Value.dbType);
            List<TH> rlist = new List<TH>();
            contextList.ForEach((contextObj) => { rlist.Add( _createRepository.Get<TH>(new object[] { contextObj })); });
            return rlist;
        }
        public TH CreateRepositoryByConnStr<T, TH>(string ConnStr)
            where T : Entity
            where TH : IRepository<T>
        {
            var contextObj = BuildDBContext.CreateJyDBContext(ConnStr, _SDBSettings.Value.dbType);
            return _createRepository.Get<TH>(new object[] { contextObj });
        }
    }
}
