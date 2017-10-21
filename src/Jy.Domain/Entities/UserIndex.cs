using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Entities
{
    public class UserIndex
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public User User { get; set; }
        public Guid DepartmentId { get; set; }
        public Role Department { get; set; }

    }
}
