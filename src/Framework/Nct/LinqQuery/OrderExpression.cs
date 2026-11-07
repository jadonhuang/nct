using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
   public class OrderExpression: Expression
    {
        public OrderType OrderType { get; set; }

        public Expression OrderBy { get; set; }

        public Expression Source { get; set; }

    }
}
