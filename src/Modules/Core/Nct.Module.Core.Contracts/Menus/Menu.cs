using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;

namespace Nct.Module.Core.Menus
{
    [Table("Core_Menu")]
    public class Menu : IEntity
    {

        [Key]
        public int Id { get; set; }


        public int ParentId { get; set; }


        public string Code { get; set; }


        public string Name { get; set; }


        public string Url { get; set; }


        public int Sort { get; set; }


        public string Icon { get; set; }


        public bool IsShow { get; set; }


        public bool IsMenu { get; set; }

        public string Path { get; set; }



    }
}
