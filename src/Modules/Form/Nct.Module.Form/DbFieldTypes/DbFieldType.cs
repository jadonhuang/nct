using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;

namespace Nct.Module.Form.DbFieldTypes
{    
    [Table("Form_DbFieldType")]
    public class DbFieldType:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public string DataType { get; set; }

       
		public string Name { get; set; }

        public string Descpt { get; set; }

        public int Length { get; set; }

        public int Precision { get; set; }


    }
}
