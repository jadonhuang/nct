using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Module.Core.AppSettings
{
    [Table("Core_AppSetting")]
    public class AppSetting : IEntity<string>
    {

        [Key]
        public string Id { get; set; }


        public string Value { get; set; }


        public string Module { get; set; }


    }
}
