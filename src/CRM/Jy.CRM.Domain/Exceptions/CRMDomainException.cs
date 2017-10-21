using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Domain.Exceptions
{
    public class CRMDomainException : Exception
    {
        public CRMDomainException()
        { }

        public CRMDomainException(string message)
            : base(message)
        { }

        public CRMDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
