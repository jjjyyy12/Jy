using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        public RoleDto GetCopy()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as RoleDto;
        }
    }
}
