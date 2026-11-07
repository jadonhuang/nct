using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public class SelectExpression : Expression
    {
        public Expression GroupBy { get; set; } 

        public Expression Select { get; set; }


    }
}
