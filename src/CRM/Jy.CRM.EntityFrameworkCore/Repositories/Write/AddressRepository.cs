using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class AddressRepository : EntityFrameworkRepositoryBase<Address,int>, IAddressRepository
    {
        protected readonly JyCRMDBContext dbContext;
        public AddressRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {
            dbContext = (JyCRMDBContext)_dbContext;
        }

        public void UpdateOrderAddress(int addressId,Guid orderId)
        {
            var order = dbContext.SecKillOrders.Find(orderId);
            order.AddressId = addressId;
            dbContext.Entry(order).State = EntityState.Modified;
        } 

        public void RemoveUserAddresss(Guid id, List<UserAddress> userAddress)
        {
            dbContext.UserAddresss.RemoveRange(dbContext.Set<UserAddress>().Where(it => it.UserId == id));
        }
        public void AddUserAddresss(List<UserAddress> userAddress)
        {
            dbContext.UserAddresss.AddRange(userAddress);
        }


        public void AddOrderAddress(Address address, Guid orderId)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var res = dbContext.Addresss.Add(address);
                    Save();
                    var order = dbContext.SecKillOrders.Find(orderId);
                    order.AddressId = res.Entity.Id;
                    dbContext.Entry(order).State = EntityState.Modified;
                    Save();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }

        }

        public void UpdateUserAddresss(Guid id, List<UserAddress> userAddress)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.UserAddresss.RemoveRange(dbContext.Set<UserAddress>().Where(it => it.UserId == id));
                    dbContext.SaveChanges();
                    dbContext.UserAddresss.AddRange(userAddress);
                    dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }

        public void BatchUpdateUserAddresss(List<Guid> userIds, List<UserAddress> userAddress)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    userIds.ForEach(x => dbContext.UserAddresss.RemoveRange(_dbContext.Set<UserAddress>().Where(it => it.UserId == x)));
                    dbContext.SaveChanges();
                    dbContext.UserAddresss.AddRange(userAddress);
                    dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }
      
        public void EditOrderAddress(Address address, Guid orderId)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var res = dbContext.Addresss.Add(address);
                    Save();
                    var order = dbContext.SecKillOrders.Find(orderId);
                    order.AddressId = res.Entity.Id;
                    dbContext.Entry(order).State = EntityState.Modified;
                    Save();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }


    }
}
