using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.DynamicQuery
{

    sealed class DynamicRowMetaObject : DynamicMetaObject
    {
        static readonly MethodInfo getValueMethod = typeof(DynamicRow).GetMethod("GetValue");
        static readonly MethodInfo setValueMethod = typeof(DynamicRow).GetMethod("SetValue");

       
        public DynamicRowMetaObject(Expression expression, BindingRestrictions restrictions, object value) : base(expression, restrictions, value)
        {
        }

        DynamicMetaObject CallMethod(MethodInfo method, Expression[] parameters)
        {
            var callMethod = new DynamicMetaObject(
               Expression.Call(
                    Expression.Convert(Expression, LimitType),
                    method,
                    parameters),
               BindingRestrictions.GetTypeRestriction(Expression, LimitType)
                );
            return callMethod;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var parameters = new Expression[]
                                 {
                                         Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }


        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var parameters = new Expression[]
                                 {
                                         Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var parameters = new Expression[]
                                 {
                                         Expression.Constant(binder.Name),
                                         value.Expression,
                                 };

            var callMethod = CallMethod(setValueMethod, parameters);

            return callMethod;
        }
    }




    public class DynamicRow : Dictionary<string, DynamicFieldValue>, IDynamicMetaObjectProvider
    {
        public DynamicRow() : base(StringComparer.OrdinalIgnoreCase) { }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicRowMetaObject(parameter, BindingRestrictions.Empty, this);
        }

        public object GetValue(string key)
        {
            var value = this[key];
            return value;
        }

        public object SetValue(string key, DynamicFieldValue value)
        {
            this[key] = value;
            return value;

        }
    }
}
