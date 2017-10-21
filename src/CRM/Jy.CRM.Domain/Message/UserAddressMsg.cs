using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.Domain.Message
{
    public class UserAddressMsg
    {
        public List<Guid> userIds;
        public List<int> AddressIds;
    }
}
