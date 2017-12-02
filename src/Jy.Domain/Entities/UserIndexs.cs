using Jy.IIndex;
using SolrNetCore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.Entities
{
    public class UserIndexs: Entity
    {
        //public Guid UserId { get; set; }
        [SolrField("name")]
        public string name { get; set; }
        [SolrField("keywords")]
        public string keywords { get; set; }
        [SolrField("depid")]
        public Guid depid { get; set; }

    }
}
