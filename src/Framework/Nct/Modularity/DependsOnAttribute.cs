using System;

namespace Nct.Modularity
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute(params Type[] dependedModuleTypes) : Attribute
    {
        public Type[] DependedModuleTypes { get; private set; } = dependedModuleTypes;

    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute<T> : Attribute where T : BaseModule
    {

    }


    public class DependsOnAttribute<T, T2> : DependsOnAttribute<T> where T : BaseModule where T2 : BaseModule
    {

    }


    public class DependsOnAttribute<T, T2, T3> : DependsOnAttribute<T> where T : BaseModule where T2 : BaseModule where T3 : BaseModule
    {

    }


}