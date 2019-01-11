using System;
using System.Collections.Generic;
using System.Text;
using Jy.IRepositories;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Jy.CRM.Domain.Dtos
{
    public class UserDto : Entity
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// 通信地址
        /// </summary>
        public string Address { get; set; }

        public List<Guid> OrderIds { get; set; }

        public List<int> AddresssIds { get; set; }

        public UserDto GetCopy()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as UserDto;
        }
    }
}
