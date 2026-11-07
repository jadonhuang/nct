using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Localization
{    

    [Table("Core_CultureResource")]
    public class CultureResource:IEntity
    {
       
		[Key]
		public int Id { get; set; }

       
		public string Name { get; set; }

       
		public string Value { get; set; }

       
		public string CultureId { get; set; }


    }
}
