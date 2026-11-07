using Nct.Extentions;
using Nct.Helpers;
using Nct.Ioc;
using Nct.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nct.Authorization
{
    public class AuthorizationChecker : IAuthorizationChecker, ITransientDependency
    {

        private const string NoLoginMsg = "请登录";
        private const string NoAuthorizeMsg = "没有权限";

        public IPermissionChecker PermissionChecker { get; set; }

        public IRoleChecker RolesChecker { get; set; }

        private readonly ISession _session;

        public AuthorizationChecker(ISession session)
        {
            _session = session;
            PermissionChecker = NullPermissionChecker.Instance;
            RolesChecker = NullRoleChecker.Instance;
        }

        public virtual void Authorize(MethodInfo method, Type type)
        {

            if (AllowAnonymous(method, type)) return;



            if (!_session.IsAuthenticated)
            {
                throw new AuthorizationException(NoLoginMsg);
            }


            var authorizeAttributes = ReflectionHelper.GetAttributesOfMemberAndType(method, type).OfType<AuthorizeExAttribute>();
            if (!authorizeAttributes.Any()) return;




            CheckRoles(authorizeAttributes);
            CheckPermissions(authorizeAttributes);

        }

        protected virtual void CheckRoles(IEnumerable<AuthorizeExAttribute> authorizeAttributes)
        {

            foreach (var authorizeAttr in authorizeAttributes)
            {

                if (authorizeAttr.Roles.IsEmpty()) continue;

                if (!RolesChecker.Authorize(authorizeAttr.Roles.SplitString()))
                    throw new AuthorizationException($"{authorizeAttr.Roles} {NoAuthorizeMsg}");

            }



        }

        protected virtual void CheckPermissions(IEnumerable<AuthorizeExAttribute> authorizeAttributes)
        {
            foreach (var authorizeAttr in authorizeAttributes)
            {
                if (authorizeAttr.Permissions.IsEmpty()) continue;
              
                if(!PermissionChecker.Authorize(authorizeAttr.Permissions.SplitString()))
                    throw new AuthorizationException($"{NoAuthorizeMsg}({authorizeAttr.Permissions})");
            }

        }


        private bool AllowAnonymous(MemberInfo memberInfo, Type type)
        {
           
            return ReflectionHelper
                .GetAttributesOfMemberAndType(memberInfo, type)
                .OfType<AnonymousAttribute>()
                .Any();
        }
    }
}
