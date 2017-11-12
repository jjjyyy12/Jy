using Jy.Domain.Entities;
using Jy.IIndex;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Domain.IIndex
{
    public interface IUserIndexsIndex : IIndex<UserIndexs>
    {
        UserIndexs InsertUserIndex(User user, bool autoSave = true);
        UserIndexs EditUserIndex(User user, bool autoSave = true);
    }
}
