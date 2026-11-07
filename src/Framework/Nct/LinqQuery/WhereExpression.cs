using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public class WhereExpression : Expression
    {
        public Expression Where { get; set; }

        public Expression Source { get; set; }

    }
}
