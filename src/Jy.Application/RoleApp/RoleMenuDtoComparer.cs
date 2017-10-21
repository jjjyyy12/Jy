using Jy.Domain.Dtos;
using System.Collections.Generic;

namespace Jy.Application.RoleApp
{
    public class RoleMenuDtoComparer : IEqualityComparer<RoleMenuDto>
    {
        public bool Equals(RoleMenuDto x, RoleMenuDto y)
        {
            return x.MenuId == y.MenuId;
        }

        public int GetHashCode(RoleMenuDto obj)
        {
            return obj.GetHashCode();
        }
    }
}
