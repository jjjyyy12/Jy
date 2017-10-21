using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Dtos
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Code不能为空。")]
        public string Code { get; set; }
        [Required(ErrorMessage = "名称不能为空。")]
        public string Name { get; set; }
        public Guid CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Remarks { get; set; }
        
    }
}
