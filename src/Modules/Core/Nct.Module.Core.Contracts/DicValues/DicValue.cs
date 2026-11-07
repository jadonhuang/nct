using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;

namespace Nct.Module.Core.DicValues
{    
    [Table("Core_DicValue")]
    public class DicValue:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public int DicId { get; set; }

       
		public string Value { get; set; }

       
		public string Name { get; set; }


        public string Color { get; set; }


        public string Descpt { get; set; }

       
		public int Sort { get; set; }


    }
}
