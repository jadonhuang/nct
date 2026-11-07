using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nct.LinqQuery
{
    public enum OrderType
    { 
       Asc,
       Desc
      
    }

    public enum SqlType { 
    
         Select,
         Delete,
         Update,
         Insert
    
    }

   
    public class Student
    {
        public string ID { get; set; }


        public string Name { get; set; }


      
    }
}
