using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CKafka
{
    public interface IConsumeConfigurator
    {
        void Configure(List<Type> consumers);
    }
}
