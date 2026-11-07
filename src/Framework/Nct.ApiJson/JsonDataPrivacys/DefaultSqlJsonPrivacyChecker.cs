using Nct.Authorization;
using Nct.DataAccess.Json;

namespace Nct.ApiJson.JsonDataPrivacys
{
    public class DefaultSqlJsonPrivacyChecker : ISqlJsonPrivacyChecker,ITransientDependency
    {
        private readonly IJsonDataPrivacyAppService _privacyAppService;
        private readonly ISession _session;
        private readonly IRoleChecker _roleChecker;
        public DefaultSqlJsonPrivacyChecker(IJsonDataPrivacyAppService privacyAppService, ISession session, IRoleChecker roleChecker)
        {
            _session = session;
            _privacyAppService = privacyAppService;
            _roleChecker = roleChecker;

        }


        private PrivacyCheckResult Check( JsonTable table, bool allowAnonymous, string allowRoles, bool enabled)
        {

            var result = new PrivacyCheckResult();

            if (!enabled)
            {

                result.Message = $"表 {table.Table} 无权限访问";
                return result;
            }    

            if (!allowAnonymous && !_session.IsAuthenticated)
            {

                result.Message = $"表 {table.Table} 不允许匿名访问";
                return result;
            }

            var allowRolesArray = allowRoles.SplitString();
            if (!allowAnonymous&&allowRolesArray.Any())
            {
                var valid = _roleChecker.Authorize(allowRolesArray);
                if (!valid)
                {
                    result.Message = $"表 {table.Table} 无权限访问";
                    return result ;
                }
            }


            result.Success = true;
            return result;

        }

        public virtual async Task<PrivacyCheckResult> Check(JsonTable table, StatementType statementType)
        {

            var result = new PrivacyCheckResult();
           
            var privacy = await _privacyAppService.GetByTableName(table.Table);
            if (privacy == null)
            {
                result.Message = $"表 {table.Table} 不存在";
                return result;
            }

            string[] notAllowedFields = null;

            switch (statementType)
            {
                case StatementType.Select:

                   var re=  Check(table, privacy.SelectAllowAnonymous, privacy.SelectAllowRoles,privacy.SelectEnabled);
                    if (!re.Success) return re;
                
                    if (!table.Fields.Any() && privacy.SelectNotAllowedFields.IsNotEmpty())
                    {
                        result.Message = $"表 {table.Table} 不允许查询所有字段";
                        return result;
                    }

                    if (table.Fields.Any() && privacy.SelectNotAllowedFields.IsNotEmpty())
                    {
                        notAllowedFields = privacy.SelectNotAllowedFields.SplitString();
                        foreach (var f in table.Fields)
                        {
                            if (notAllowedFields.Any(m => m.IgnoreCaseEqual(f.Name)))
                            {
                                result.Message = $"字段 {table.Table}.{f.Name} 不允许查询";
                                return result;
                            }

                        }

                    }

                    break;

                case StatementType.Insert:
                     re = Check(table, privacy.InsertAllowAnonymous, privacy.InsertAllowRoles, privacy.InsertEnabled);
                    if (!re.Success) return re;

                    notAllowedFields = privacy.InsertNotAllowedFields.SplitString();
                    if (notAllowedFields.Any())
                    {
                        foreach (var f in table.FieldValues)
                        {
                            if (notAllowedFields.Any(m => m.IgnoreCaseEqual(f.Name)))
                            {
                                result.Message = $"字段 {table.Table}.{f.Name} 不允许插入";
                                return result;
                            }

                        }

                    }
                    break;

                case StatementType.Update:
                    re = Check(table, privacy.UpdateAllowAnonymous, privacy.UpdateAllowRoles, privacy.UpdateEnabled);
                    if (!re.Success) return re;

                    notAllowedFields = privacy.UpdateNotAllowFields.SplitString();
                    if (notAllowedFields.Any())
                    {
                        foreach (var f in table.FieldValues)
                        {
                            if (notAllowedFields.Any(m => m.IgnoreCaseEqual(f.Name)))
                            {
                                result.Message = $"字段 {table.Table}.{f.Name} 不允许修改";
                                return result;
                            }

                        }
                    }



                    break;

                case StatementType.Delete:
                    re = Check(table, privacy.DeleteAllowAnonymous, privacy.DeleteAllowRoles, privacy.DeleteEnabled);
                    if (!re.Success) return re;
                    break;


            }

            result.Success = true;
            return result;


        }

    }
}
