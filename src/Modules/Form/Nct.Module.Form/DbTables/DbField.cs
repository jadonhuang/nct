using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;
using Nct.ObjectMapper;

namespace Nct.Module.Form.DbTables
{
    [Table("Form_DbField")]
    public class DbField : IEntity, ISoftDelete
    {

        [Key]
        public int Id { get; set; }


        public string ColName { get; set; }


        public string FieldType { get; set; }


        public int TableId { get; set; }

        public bool IsSys{ get; set; }

        public bool IsDeleted { get; set; }


    }

    [Map(typeof(FieldInfo))]
    public class DbFieldDto
    {


        public string Name { get; set; }

        [Required]
        public string FieldType { get; set; }


        public string Remark { get; set; }

        public int Length { get; set; }

        public bool IsKey { get; set; }

        public bool IsNullable { get; set; } = true;



    }




}
