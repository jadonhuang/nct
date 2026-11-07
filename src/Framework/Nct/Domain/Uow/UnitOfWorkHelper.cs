using Nct.Helpers;
using System;
using System.Reflection;

namespace Nct.Domain.Uow
{
    public static class UnitOfWorkHelper
    {


        public static bool IsUnitOfWorkType(TypeInfo implementationType)
        {
           
            if (HasUnitOfWorkAttribute(implementationType) || ReflectionHelper.HasAnyAttribute<UnitOfWorkAttribute>(implementationType))
            {
                return true;
            }

            if (typeof(IUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(implementationType))
            {
                return true;
            }

            return false;
        }

        public static bool IsUnitOfWorkMethod(MethodInfo methodInfo, out UnitOfWorkAttribute unitOfWorkAttribute)
        {

            Check.NotNull(methodInfo, nameof(methodInfo));

             unitOfWorkAttribute = GetUnitOfWorkAttributeOrNull(methodInfo);

            if (unitOfWorkAttribute != null)
            {
                return unitOfWorkAttribute.Enable;

            }

            if (methodInfo.DeclaringType != null)
            {
                if (typeof(IUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(methodInfo.DeclaringType))
                {
                    unitOfWorkAttribute = null;
                    return true;
                }
            }

            unitOfWorkAttribute = null;
            return false;
        }


        public static UnitOfWorkAttribute GetUnitOfWorkAttributeOrNull(MethodInfo methodInfo)
        {
            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<UnitOfWorkAttribute>(methodInfo);
        }

   
        private static bool HasUnitOfWorkAttribute(MemberInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
        }


        public static UnitOfWorkOptions CreateOptions(UnitOfWorkAttribute unitOfWorkAttribute, string methodName)
        {

            unitOfWorkAttribute ??= new UnitOfWorkAttribute();

            unitOfWorkAttribute.IsTransactional ??= !methodName.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase);

            UnitOfWorkOptions options = unitOfWorkAttribute.CreateOptions();
            return options;
        }



    }
}
