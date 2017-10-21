using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Dtos
{
    public class UserToken
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public DateTime LoginTime { get; set; }

        public List<Guid> RoleIds { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public string Email { get; set; }
        public string Mobile { get; set; }
    }
}
