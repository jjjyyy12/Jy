using Jy.Domain.Entities;
using Jy.IIndex;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Domain.IIndex
{
    public interface IUserIndexsIndexRead : IIndexRead<UserIndexs>
    {
        UserIndexs CheckUserIndex(string userName, string password);
        List<UserIndexs> GetUserIndexList(ICollection<KeyValuePair<string, string>> wheres);

    }
}
