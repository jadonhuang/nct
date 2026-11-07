using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.DbTables
{
    public class FieldInfo
    {
        public string Name { get; set; }

        public string DataType { get; set; }

        public int Length { get; set; }

        public bool IsIdentity { get; set; }

       public bool  IsKey { get; set; }

        public bool IsNullable { get; set; }

        public string Remark { get; set; }

        public string FieldType { get; set; }

        public string FieldTypeName { get; set; }

        public bool IsSys { get; set; }

    }
}
