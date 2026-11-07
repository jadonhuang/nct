using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.LinqQuery
{
    public class EntityObject
    {

        public string Name { get; set; }

        public string TableName { get; set; }

        public string Schema { get; set; }


        public Type EntityType { get; set; }


        public List<EntityField> Fields { get; set; } = new List<EntityField>();



    }


    public class EntityField
    {
        public string Name { get; set; }

        public string ColumnName { get; set; }

        public bool IsKey { get; set; }


        public bool IsIdentity { get; set; }


        public Type Type { get; set; }



    }



}
