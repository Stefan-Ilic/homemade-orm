using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ORM.Attributes;

namespace ORM
{
    public abstract class OrmUtilities
    {
        public static Dictionary<string, Type> GetColumns(Type tableType)
        {
            var list = new Dictionary<string, Type>();
            var properties = tableType.GetProperties();

            if (properties == null) return list;

            foreach (var property in properties)
            {
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                list.Add(columnAttribute != null ? columnAttribute.ColumnName : property.Name,
                    property.PropertyType);
            }
            return list;
        }

        public static string GetTableName(Type tableType)
        {
            var className = tableType.Name;
            var tableAttributeName = tableType.GetCustomAttribute<TableAttribute>()?.TableName;
            return tableAttributeName ?? className;
        }
    }
}
