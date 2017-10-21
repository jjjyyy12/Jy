using Jy.CRM.Domain.Entities;
using Jy.IRepositories;
using System;
using System.Collections.Generic;

namespace Jy.CRM.Domain.IRepositories
{
    public interface IAddressRepository : IRepository<Address, int>
    {
        void UpdateOrderAddress(int addressId, Guid orderId);
        void RemoveUserAddresss(Guid id, List<UserAddress> userAddress);
        void AddUserAddresss(List<UserAddress> userAddress);

        void UpdateUserAddresss(Guid id, List<UserAddress> userAddress);
        void BatchUpdateUserAddresss(List<Guid> userIds, List<UserAddress> userAddress);
        void AddOrderAddress(Address address, Guid orderId);
        void EditOrderAddress(Address address, Guid orderId);
    }
}
