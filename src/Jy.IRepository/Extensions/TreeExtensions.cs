using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jy.IRepositories
{
    public static class TreeExtensions
    {
        /// <summary>
        /// Turn the tree shape Collection into one single root tree shape structure
        /// </summary>
        /// <typeparam name="T">Tree Shape Entity</typeparam>
        /// <param name="items">Tree Shape Collection</param>
        /// <returns>Tree shape structure object</returns>
        public static TreeEntity<T> ToSingleRoot<T>(this IEnumerable<TreeEntity<T>> items) where T : Entity
        {
            var all = items.ToList();
            if (!all.Any())
            {
                return null;
            }
            var top = all.Where(x => x.ParentId == null).ToList();
            if (top.Count > 1)
            {
                throw new Exception("There is more than 1 root of tree");
            }
            if (top.Count == 0)
            {
                throw new Exception("Can't find the root of tree");
            }
            TreeEntity<T> root = top.Single();

            Action<TreeEntity<T>> findChildren = null;
            findChildren = current =>
            {
                var children = all.Where(x => x.ParentId.Equals(current.Id.ToString())).ToList();
                foreach (var child in children)
                {
                    findChildren(child);
                }
                current.Children = children as ICollection<T>;
            };

            findChildren(root);

            return root;
        }

        /// <summary>
        /// 把树形结构数据的集合转化成多个根结点的树形结构数据
        /// </summary>
        /// <typeparam name="T">树形结构实体</typeparam>
        /// <param name="items">树形结构实体的集合</param>
        /// <returns>多个树形结构实体根结点的集合</returns>
        public static List<TreeEntity<T>> ToMultipleRoots<T>(this IEnumerable<TreeEntity<T>> items) where T : TreeEntity<T>
        {
            List<TreeEntity<T>> roots;
            var all = items.ToList();
            if (!all.Any())
            {
                return null;
            }
            var top = all.Where(x => x.ParentId == null).ToList();
            if (top.Any())
            {
                roots = top;
            }
            else
            {
                throw new Exception("Can't find the root of tree");
            }

            Action<TreeEntity<T>> findChildren = null;
            findChildren = current =>
            {
                var children = all.Where(x => x.ParentId.Equals(current.Id.ToString())).ToList();
                foreach (var child in children)
                {
                    findChildren(child);
                }
                current.Children = children as ICollection<T>;
            };

            roots.ForEach(findChildren);

            return roots;
        }

        /// <summary>
        /// 作为父节点, 取得树形结构实体的祖先ID串
        /// </summary>
        /// <typeparam name="T">树形结构实体</typeparam>
        /// <param name="parent">父节点实体</param>
        /// <returns></returns>
        public static string GetAncestorIdsAsParent<T>(this T parent) where T : TreeEntity<T>
        {
            return string.IsNullOrEmpty(parent.AncestorIds) ? parent.Id.ToString() : (parent.AncestorIds + "-" + parent.Id);
        }
    }
}
