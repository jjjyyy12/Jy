using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class AddressRepository : JyRepositoryBase<Address,int, JyCRMDBContext>, IAddressRepository
    {
        public AddressRepository(JyCRMDBContext dbcontext) : base(dbcontext, dbcontext)
        {

        }

        public void UpdateOrderAddress(int addressId,Guid orderId)
        {
            var order = _dbContext.SecKillOrders.Find(orderId);
            order.AddressId = addressId;
            _dbContext.Entry(order).State = EntityState.Modified;
        } 

        public void RemoveUserAddresss(Guid id, List<UserAddress> userAddress)
        {
            _dbContext.UserAddresss.RemoveRange(_dbContext.Set<UserAddress>().Where(it => it.UserId == id));
        }
        public void AddUserAddresss(List<UserAddress> userAddress)
        {
            _dbContext.UserAddresss.AddRange(userAddress);
        }


        public void AddOrderAddress(Address address, Guid orderId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var res = _dbContext.Addresss.Add(address);
                    Save();
                    var order = _dbContext.SecKillOrders.Find(orderId);
                    order.AddressId = res.Entity.Id;
                    _dbContext.Entry(order).State = EntityState.Modified;
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.UserAddresss.RemoveRange(_dbContext.Set<UserAddress>().Where(it => it.UserId == id));
                    _dbContext.SaveChanges();
                    _dbContext.UserAddresss.AddRange(userAddress);
                    _dbContext.SaveChanges();
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    userIds.ForEach(x => _dbContext.UserAddresss.RemoveRange(_dbContext.Set<UserAddress>().Where(it => it.UserId == x)));
                    _dbContext.SaveChanges();
                    _dbContext.UserAddresss.AddRange(userAddress);
                    _dbContext.SaveChanges();
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var res = _dbContext.Addresss.Add(address);
                    Save();
                    var order = _dbContext.SecKillOrders.Find(orderId);
                    order.AddressId = res.Entity.Id;
                    _dbContext.Entry(order).State = EntityState.Modified;
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
