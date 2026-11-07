using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Modularity
{
    public class ModuleCollection : List<ModuleInfo>
    {
        public Type StartupModuleType { get; }

        public ModuleCollection(Type startupModuleType)
        {
            StartupModuleType = startupModuleType;
        }

        public void EnsureKernelModuleToBeFirst(List<ModuleInfo> list = null)
        {
            if (list == null) list = this;

            var index = list.FindIndex(m => m.Type == typeof(KernelModule));
            if (index <= 0)
            {
                return;
            }

            var kernelModule = list[index];
            list.RemoveAt(index);
            list.Insert(0, kernelModule);


        }


        public void EnsureStartupModuleToBeLast(List<ModuleInfo> list = null)
        {
            if (list == null) list = this;

            var index = list.FindIndex(m => m.Type == StartupModuleType);
            if (index == list.Count - 1)
            {
                return;
            }

            var startupModule = list[index];
            list.RemoveAt(index);
            list.Add(startupModule);

        }


        public List<ModuleInfo> GetSortedListByDependency()
        {
            var sortedModules = SortByDependencies(this, x => x.Dependencies);

            EnsureKernelModuleToBeFirst(sortedModules);
            EnsureStartupModuleToBeLast(sortedModules);
            return sortedModules;
        }


        private List<T> SortByDependencies<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {

            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }


        private void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }


    }
}
