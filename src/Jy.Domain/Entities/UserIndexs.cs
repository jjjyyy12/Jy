using Jy.IIndex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Entities
{
    public class UserIndexs: Entity
    {
        //public Guid UserId { get; set; }
        public string name { get; set; }
        public string keywords { get; set; }
        public Guid depid { get; set; }

    }
}
