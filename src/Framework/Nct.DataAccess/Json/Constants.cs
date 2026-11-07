using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{


    public enum StatementType
    {
        Select,

        Insert,

        Update,

        Delete,

        None,

    }

    public enum LogicType
    {

        And = 1,
        Or = 2,

    }

    public enum JoinType
    {

        InnerJoin,
        LeftJoin

    }

    public enum AssociateType
    {
        Equal,
        Like,
        ReverseLike,

    }

    public class Constants
    {



        public readonly static List<string> SqlOperators = new List<string> { ">=", "<=", ">", "<", "!=", "=", "<>" };

        public readonly static List<string> ValueOperators = new List<string> { "+", "-", "*", "/" };


        public static class SqlKeys
        {
            public const string IN = "in";
            public const string NOTIN = "not in";
            public const string JOIN = "join";
            public const string ISNULL = "is null";
            public const string ISNOTNULL = "is not null";
            public const string LIKE = "like";


        }

        public static class JsonKeys
        {


            public const string ARRAY = "[]";
            public const string ID = "id";
            public const string CONDITION = "{}";
            public const string COLUMN = "@column";
            public const string GROUP = "@group";
            public const string HAVING = "@having";
            public const string ORDER = "@order";
            public const string BETWEEN = "%";
            public const string LIKE = "$";
            public const string AND = "&";
            public const string OR = "|";
            public const string NOT = "!";
            public const string PAGE = "page";
            public const string COUNT = "count";
            public const string TOTAL = "total";
            public const string QUERY = "query";
            public const string RELATED = "@";
            public const string COMBINE = "@combine";
            public const string AUTOINCREM = "^";
            public const string JOIN = "~";
            public const string LEFTJOIN = ">~";
            public const string STATEMENT = "@statement";

        }


    }



}
