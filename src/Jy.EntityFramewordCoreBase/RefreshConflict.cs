using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.EntityFramewordCoreBase
{
    //当提交时定义是否为数据库或者客户端或者数据库和客户端数据合并
    public enum RefreshConflict
    {
        StoreWins,

        ClientWins,

        MergeClientAndStore
    }
}
