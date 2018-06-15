using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Cache.HashAlgorithms
{
    public interface IHashAlgorithm
    {
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <param name="item">字符串</param>
        /// <returns>返回哈希值</returns>
        int Hash(string item);
    }
}
