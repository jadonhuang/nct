using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Nct.Application
{
    public class PageResult<T>: PageResult
    {
         
        public List<T> Rows { get; set; }

    }


    public class PageTResult<T> : PageResult
    {

        public T Rows { get; set; }

        public object Data { get; set; }

    }


    public class PageTableResult : PageTResult<DataTable>
    {


    }


    public class PageResult
    {

        public long Total { get; set; }
    
        public int PageIndex { get; set; }

        public int PageSize { get; set; }


        public int PageNum
        {
            get
            {

                return (int)Math.Ceiling((double)Total / PageSize);


            }

        }

    }

}
