using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IFile
{
    /// <summary>
    /// 存储类型
    /// </summary>
    public enum StorageTypeEnum
    {
        /// <summary>
        /// ECS服务器本地磁盘
        /// </summary>
        DISK = 1,

        /// <summary>
        /// 阿里云OSS存储
        /// </summary>
        OSS = 2,

        /// <summary>
        /// LINUX磁盘存储
        /// </summary>
        LINUXDISK = 3
    }
}
