using Nct.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Module.Form.DataSources
{

    public class SqlExtraInfo
    {
        public string OrderBy { get; set; }

        public bool IsSimple { get; set; }

        public bool IsNoWhere { get; set; }

    }


    [Table("Form_DataSource")]
    public class DataSource : IEntity,ISoftDelete, IHasCreateTime,IHasCreateUser
    {

        [Key]
        public int Id { get; set; }


        public string Name { get; set; }


        public string Code { get; set; }


        public int Cate { get; set; }


        public DateTime CreateTime { get; set; }


        public bool IsDeleted { get; set; }


        public string CreateUser { get; set; }


        public string Descpt { get; set; }


        public int Type { get; set; }


        public string Data { get; set; }

        [NotMapped]
        public bool IsScript
        {

            get
            {

                return Type == 2;
            }
        }


    }
}
