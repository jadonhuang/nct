using System;


namespace Nct.ObjectMapper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapAttribute : Attribute
    {
        public Type TargetType { get; }
        public MapAttribute(Type targetType)
        {
            TargetType = targetType;

        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapToAttribute : Attribute
    {
        public Type TargetType { get; }
        public MapToAttribute(Type targetType)
        {
            TargetType = targetType;

        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapFromAttribute : Attribute
    {

        public Type SourceType { get; }
        public MapFromAttribute(Type sourceType)
        {
            SourceType = sourceType;

        }
    }
}
