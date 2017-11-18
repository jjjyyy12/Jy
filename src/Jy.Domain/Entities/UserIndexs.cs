using Jy.IIndex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Entities
{
    public class UserIndexs: Entity
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid DepartmentId { get; set; }

    }
}
