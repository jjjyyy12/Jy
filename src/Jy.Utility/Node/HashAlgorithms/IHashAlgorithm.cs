using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Utility.Node
{
    /// <summary>
    /// 一致性哈希的抽象接口
    /// </summary>
    public interface IHashAlgorithm
    {
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <param name="item">字符串</param>
        /// <returns>返回哈希值</returns>
        /// <remarks>
        /// 用法：
        /// private readonly IHashAlgorithm _hashAlgorithm;
        ///  _hashAlgorithm.Hash(parameter.ToString());
        /// </remarks>
        int Hash(string item);
    }
}
