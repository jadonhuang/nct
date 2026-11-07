using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Module.Core.Dictionarys
{    
    [Table("Core_Dictionary")]
    public class Dictionary:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public int ParentId { get; set; }

       
		public string Code { get; set; }

       
		public string Name { get; set; }

       
		public string Descpt { get; set; }

       
		public string Path { get; set; }

       
		public int Sort { get; set; }

       
		public int Depth { get; set; }




    }
}
