using Jy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Message
{
    public class UserRoleMsg
    {
        public List<Guid> userIds;
        public List<Guid> roleIds;
    }
}
