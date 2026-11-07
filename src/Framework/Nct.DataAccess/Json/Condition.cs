using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{

    public interface ICondition { }

    public class Condition : ICondition { 
        
         public LogicType Type { get; set; } = LogicType.And;

        public string Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }

        public string FunExpression { get; set; }


    }

    public class ConditionCollection : ICondition
    {
        public List<Condition> Conditions { get; set; } = new();

    }


    public class JsonCondition
    {

      
        public bool IsWhere { get; set; } = true;

        public string Tag { get; set; }

        public List<ICondition> Conditions { get; set; } = new();

    }




    public class ConditionSql
    {
        public string Tag { get; set; }
        public string  Sql { get; set; }

    }


}
