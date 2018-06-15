﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Jy.Cache.HashAlgorithms
{
    /// <summary>
    /// 针对<see cref="T"/>哈希算法实现
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class ConsistentHash<T>
    {
        #region 字段
        private readonly SortedDictionary<int, T> _ring = new SortedDictionary<int, T>();
        private int[] _nodeKeysInRing = null;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly int _virtualNodeReplicationFactor = 1000;
        #endregion

        public ConsistentHash(IHashAlgorithm hashAlgorithm)
        {
            _hashAlgorithm = hashAlgorithm;
        }

        public ConsistentHash(IHashAlgorithm hashAlgorithm, int virtualNodeReplicationFactor)
            : this(hashAlgorithm)
        {
            _virtualNodeReplicationFactor = virtualNodeReplicationFactor;
        }

        #region 属性
        /// <summary>
        /// 复制哈希节点数
        /// </summary>
        public int VirtualNodeReplicationFactor
        {
            get { return _virtualNodeReplicationFactor; }
        }
        #endregion

        /// <summary>
        /// 初始化节点服务器
        /// </summary>
        /// <param name="nodes">节点</param>
        public void Initialize(IEnumerable<T> nodes)
        {
            foreach (var node in nodes)
            {
                AddNode(node);
            }
            _nodeKeysInRing = _ring.Keys.ToArray();
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点</param>
        public void Add(T node)
        {
            AddNode(node);
            _nodeKeysInRing = _ring.Keys.ToArray();
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node">节点</param>
        public void Remove(T node)
        {
            RemoveNode(node);
            _nodeKeysInRing = _ring.Keys.ToArray();
        }

        /// <summary>
        /// 通过哈希算法计算出对应的节点
        /// </summary>
        /// <param name="item">值</param>
        /// <returns>返回节点</returns>
        public T GetItemNode(string item)
        {
            var hashOfItem = _hashAlgorithm.Hash(item);
            var nearestNodePosition = GetClockwiseNearestNode(_nodeKeysInRing, hashOfItem);
            return _ring[_nodeKeysInRing[nearestNodePosition]];
        }

        public IEnumerable<T> GetNodes()
        {
            return _ring.Values.Distinct().ToList();
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点</param>
        private void AddNode(T node)
        {
            for (var i = 0; i < _virtualNodeReplicationFactor; i++)
            {
                var hashOfVirtualNode = _hashAlgorithm.Hash(node.GetHashCode().ToString(CultureInfo.InvariantCulture) + i);
                _ring[hashOfVirtualNode] = node;
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node">节点</param>
        private void RemoveNode(T node)
        {
            for (var i = 0; i < _virtualNodeReplicationFactor; i++)
            {
                var hashOfVirtualNode = _hashAlgorithm.Hash(node.GetHashCode().ToString() + i);
                if (_ring.ContainsKey(hashOfVirtualNode))
                    _ring.Remove(hashOfVirtualNode);
            }
        }


        /// <summary>
        /// 顺时针查找对应哈希的位置
        /// </summary>
        /// <param name="keys">键集合数</param>
        /// <param name="hashOfItem">哈希值</param>
        /// <returns>返回哈希的位置</returns>
        private int GetClockwiseNearestNode(int[] keys, int hashOfItem)
        {
            var begin = 0;
            var end = keys.Length - 1;
            if (keys[end] < hashOfItem || keys[0] > hashOfItem)
            {
                return 0;
            }
            var mid = begin;
            while ((end - begin) > 1)
            {
                mid = (end + begin) / 2;
                if (keys[mid] >= hashOfItem) end = mid;
                else begin = mid;
            }
            return end;
        }
    }
}
