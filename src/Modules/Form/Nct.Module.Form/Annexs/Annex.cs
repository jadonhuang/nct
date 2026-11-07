using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Nct.Domain.Entities;


namespace Nct.Module.Form.Annexs
{    
    [Table("Form_Annex")]
    public class Annex:IEntity<string>,IHasCreateTime
    {
       
		[Key]
		public string Id { get; set; }

       
		public string Name { get; set; }

       
		public long Size { get; set; }

       
		public string ContentType { get; set; }

		public string Path { get; set; }

       
		public string Extension { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
