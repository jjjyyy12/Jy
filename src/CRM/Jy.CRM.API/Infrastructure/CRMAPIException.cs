using System;

namespace Jy.CRM.API.Infrastructure
{
    public class CRMAPIException : Exception
    {
        public CRMAPIException()
        { }

        public CRMAPIException(string message)
            : base(message)
        { }

        public CRMAPIException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
