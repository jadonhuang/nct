using Nct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Extentions
{
    public static class TreeExtensions
    {
        public static IList<TModel> ToTree<TType,TModel>(this IEnumerable<TModel> list) where TModel:ITreeNode<TType, TModel> 
        {

            var childMap = new Dictionary<TType, IList<TModel>>();
            var parentMap = new Dictionary<TType, int>();

            var treeList = new List<TModel>();

            if (!list.Any())
            {
                return treeList;
            }

            foreach (var node in list)
            {
                if (parentMap.ContainsKey(node.Id)) continue;   //有可能节点有重复数据

                childMap.TryAdd(node.ParentId, new List<TModel>());

                childMap[node.ParentId].Add(node);

                parentMap.Add(node.Id, 0);

                childMap.TryAdd(node.Id, new List<TModel>());

                node.Children = childMap[node.Id];

            }
            foreach (var item in childMap)
            {
                if (!parentMap.ContainsKey(item.Key))
                {
                    treeList.AddRange(item.Value);
                }
            }
            return treeList;
        }

    


    }
}
