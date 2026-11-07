using Nct.Extentions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Nct.LinqQuery
{
    public class EntityCache
    {
        private static readonly ConcurrentDictionary<Type, EntityObject> Entities = new ConcurrentDictionary<Type, EntityObject>();

        private const string DefaultKey = "id";

        public static EntityObject GetOrAdd(Type type)
        {

            if (!Entities.TryGetValue(type, out EntityObject entityObj))
            {
                entityObj = new EntityObject();
                entityObj.EntityType = type;
                entityObj.Name = entityObj.TableName = type.Name;

                var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), true).Cast<TableAttribute>().FirstOrDefault();
                if (tableAttr != null)
                {
                    entityObj.TableName = tableAttr.Name;
                }

                var props = type.GetProperties();

                foreach (var prop in props)
                {

                    var propAttrs = prop.GetCustomAttributes(false);

                    if (propAttrs.Any(m => m.GetType() == typeof(NotMappedAttribute))) continue;

                    EntityField entityField = new EntityField();
                    entityField.Name = entityField.ColumnName = prop.Name;
                    entityField.Type = prop.PropertyType;

                    var colAttr = propAttrs.Where(m => m.GetType() == typeof(ColumnAttribute)).Cast<ColumnAttribute>().FirstOrDefault();
                    if (colAttr != null)
                    {
                        entityField.ColumnName = colAttr.Name;
                    }

                    if (propAttrs.Any(m => m.GetType() == typeof(KeyAttribute)) || prop.Name.IgnoreCaseEqual(DefaultKey))
                    {
                        entityField.IsKey = true;
                    }

                    var genAttr = propAttrs.Where(m => m.GetType() == typeof(DatabaseGeneratedAttribute)).Cast<DatabaseGeneratedAttribute>().FirstOrDefault();

                    if (genAttr != null && genAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                    {
                        entityField.IsIdentity = true;
                    }
                    else if ((genAttr == null || genAttr.DatabaseGeneratedOption != DatabaseGeneratedOption.None) && prop.Name.IgnoreCaseEqual(DefaultKey))
                    {
                        if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long))
                            entityField.IsIdentity = true;
                    }

                    entityObj.Fields.Add(entityField);

                }

                Entities.TryAdd(type, entityObj);

            }

            return entityObj;


        }


    }
}
