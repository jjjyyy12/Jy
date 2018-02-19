﻿using Jy.CRM.Domain.IRepositories;
using Jy.IRepositories;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using Jy.EntityFramewordCoreBase.Repositories;

namespace Jy.CRM.EntityFrameworkCore.Repositories
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
                _createRepository.AddConfig<IAddressRepository, AddressRepository>();
                _createRepository.AddConfig<ICommodityRepository, CommodityRepository>();
                _createRepository.AddConfig<IPaymentRepository, PaymentRepository>();
                _createRepository.AddConfig<ISecKillOrderRepository, SecKillOrderRepository>();
            }
        }
        private  JyCRMDBContext getContext(string Id)
        {
            return BuildDBContext.CreateJyDBContextFromId(Id, _SDBSettings.Value.connectionKeyList, _SDBSettings.Value.connectionList,_SDBSettings.Value.defaultConnectionString,_SDBSettings.Value.dbType);
        }
        private EntityFrameworkRepositoryContext getRepositoryContext(JyCRMDBContext context)
        {
            return new EntityFrameworkRepositoryContext(context);
        }
        public HashSet<string> GetConnectionStrings()
        {
            return BuildDBContext.GetConnectionStrings(_SDBSettings.Value.connectionList, _SDBSettings.Value.defaultConnectionString, _SDBSettings.Value.dbType);
        }
        public TH CreateRepository<T, TH>(string Id)
            where T : Entity
            where TH : IRepository<T>
        {
            var contextObj = getContext(Id);
            var repositoryContext = getRepositoryContext(contextObj);
            return _createRepository.Get<TH>(new object[] { repositoryContext });
        }
        public TH CreateDefaultRepository<T, TH>()
           where TH : IRepository<T>
           where T : Entity
        {
            var contextObj = BuildDBContext.CreateJyDBContext(_SDBSettings.Value.defaultConnectionString, _SDBSettings.Value.dbType);
            var repositoryContext = getRepositoryContext(contextObj);
            return _createRepository.Get<TH>(new object[] { repositoryContext });
        }
        //得到所有分库的Repository
        public List<TH> CreateAllRepository<T, TH>()
            where T : Entity
            where TH : IRepository<T>
        {
            var contextList = BuildDBContext.CreateAllJyDBContext(_SDBSettings.Value.connectionList, _SDBSettings.Value.dbType);
            List<TH> rlist = new List<TH>();
            contextList.ForEach((contextObj) => {
                var repositoryContext = getRepositoryContext(contextObj);
                rlist.Add(_createRepository.Get<TH>(new object[] { repositoryContext }));
            });
            return rlist;
        }

        public TH CreateRepositoryByConnStr<T, TH>(string ConnStr)
            where T : Entity
            where TH : IRepository<T>
        {
            var contextObj = BuildDBContext.CreateJyDBContext(ConnStr, _SDBSettings.Value.dbType);
            var repositoryContext = getRepositoryContext(contextObj);
            return _createRepository.Get<TH>(new object[] { repositoryContext });
        }
    }
}
