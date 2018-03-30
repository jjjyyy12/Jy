using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jy.DapperBase.Repositories.Extensions
{
    public class InvalidCacheItemException : Exception
    {
        public InvalidCacheItemException(string message) : base(message)
        {
        }
    }
}
