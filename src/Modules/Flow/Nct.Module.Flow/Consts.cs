using System.ComponentModel;

namespace Nct.Module.Flow
{
    public static class Consts
    {


        public static class VariableType
        {

            [Description("字符")]
            public const string String = "String";

            [Description("数字")]
            public const string Number = "Number";

            [Description("数字数组")]
            public const string ArrayNumber = "Array[Number]";

            [Description("字符数组")]
            public const string ArrayString = "Array[String]";

            [Description("对象数组")]
            public const string ArrayObject = "Array[Object]";

            [Description("对象")]
            public const string Object = "Object";
        }

        public static class HttpBodyType
        {
         
            public const string None = "none";
         
            public const string FormData = "formdata";

            public const string FormUrlEncoded = "x-www-form-urlencoded";

            public const string Raw = "raw";

            public const string Json = "json";

          

        }

        public static class Operators
        {
          
            public const string Greater = ">";
            public const string Less = "<";
            public const string Equal = "=";
            public const string NotEqual = "<>";
            public const string GreaterEqual = ">=";
            public const string LessEqual = "<=";
            public const string Contains = "contain";
            public const string NotContains = "notcontain";
            public const string Empty = "empty";
            public const string NotEmpty = "notempty";
            public const string Starts = "starts";
            public const string Ends = "ends";

        }



        public static class LogicalOperator
        {

            public const string And = "AND";

            public const string Or = "OR";

      
        }



        public static class AuditType
        {

            [Description("会签")]
            public const int All = 1;

            [Description("任何")]
            public const int Any = 2;


            [Description("顺序")]
            public const int Order = 3;

        }

        public static class AuditMemberType
        {

            [Description("用户")]
            public const int User = 1;

            [Description("角色")]
            public const int Role = 2;

            [Description("领导")]
            public const int Leader = 3;

        }


        public static class TaskStatus
        {

            [Description("未开始")]
            public const int NotStart = 0;

            [Description("已开始")]
            public const int Started = 1;


            [Description("同意")]
            public const int Passed = 2;


            [Description("拒绝")]
            public const int NotPassed = 3;

        }

        public static class AuditStatus
        {

            [Description("未开始")]
            public const int NotStart = 0;

            [Description("同意")]
            public const int Passed = 1;


            [Description("拒绝")]
            public const int NotPassed = 2;

            [Description("未分配")]
            public const int Null = 99;

        }

        public static class FlowStatus
        {

            [Description("未开始")]
            public const int NotStart = 0;

            [Description("已开始")]
            public const int Started = 1;


            [Description("通过")]
            public const int Passed = 2;


            [Description("未通过")]
            public const int NotPassed = 3;

            [Description("驳回")]
            public const int Reject = 4;


        }

        public static class NodeType
        {

            [Description("开始")]
            public const string Start = "start";

            [Description("任务")]
            public const string Task = "task";

            [Description("结束")]
            public const string End = "end";

            [Description("条件")]
            public const string Case = "case";

            [Description("脚本")]
            public const string Script = "script";

            [Description("组循环开始")]
            public const string GroupStart = "groupstart";
        }


        public static class ExpireHandle
        {
            [Description("通知")]
            public const int Notify = 1;

            [Description("通过")]
            public const int Pass = 2;

            [Description("拒绝")]
            public const int Refuse = 3;

        }

        public static class TimeUnit
        {
            [Description("天")]
            public const int Day = 1;

            [Description("小时")]
            public const int Hour = 2;

            [Description("分钟")]
            public const int Minute = 3;

        }

    }
}
