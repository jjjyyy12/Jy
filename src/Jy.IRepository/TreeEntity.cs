using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IRepositories
{
    public class TreeEntity<T> : Entity where T : Entity
    {
        public string ParentId { get; set; }
        public string AncestorIds { get; set; }
        public int Level => AncestorIds?.Split('-').Length ?? 0;
        public T Parent { get; set; }
        public ICollection<T> Children { get; set; }
    }
}
