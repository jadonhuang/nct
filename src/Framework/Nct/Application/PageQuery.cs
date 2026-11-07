using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Application
{
    public class PageQuery<TQuery> : IPageQuery
    {

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public TQuery Query { get; set; }

        public string OrderBy { get; set; }
    }



}
