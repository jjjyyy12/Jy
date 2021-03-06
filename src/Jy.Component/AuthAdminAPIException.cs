﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Component
{
    public class AuthAdminAPIException : Exception
    {
        public AuthAdminAPIException()
        { }

        public AuthAdminAPIException(string message)
            : base(message)
        { }

        public AuthAdminAPIException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
