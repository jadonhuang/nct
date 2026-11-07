using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Localization
{    

    [Table("Core_Culture")]
    public class Culture:IEntity<string>
    {
       
		[Key]
		public string Id { get; set; }

       
		public string Name { get; set; }


    }
}
