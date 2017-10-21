using Jy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Message
{
    public class RoleMenuMsg
    {
        public Guid Id;
        public List<Guid> menuIds;
    }
}
