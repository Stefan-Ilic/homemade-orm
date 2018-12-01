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
        public static Dictionary<string, (Type, object)> GetColumns(Type tableType)
        {
            var dictionary = new Dictionary<string, (Type type, object value)>();

            var properties = tableType.GetProperties();
            if (properties == null) return dictionary;

            foreach (var property in properties)
            {
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

                dictionary.Add(columnAttribute != null ? columnAttribute.ColumnName : property.Name,
                    (property.PropertyType, null));
            }

            return dictionary;
        }

        public static Dictionary<string, (Type, object)> GetColumns(object objectToInsert)
        {
            var dictionary = new Dictionary<string, (Type type, object value)>();

            var properties = objectToInsert.GetType().GetProperties();
            if (properties == null) return dictionary;

            foreach (var property in properties)
            {
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                dictionary.Add(columnAttribute != null ? columnAttribute.ColumnName : property.Name,
                    (property.PropertyType, property.GetValue(objectToInsert)));
            }

            return dictionary;
        }

        public static string GetTableName(Type tableType)
        {
            var className = tableType.Name;
            var tableAttributeName = tableType.GetCustomAttribute<TableAttribute>()?.TableName;
            return tableAttributeName ?? className;
        }

        public static PropertyInfo GetPrimaryKeyProperty(Type tableType)
        {
            var properties = tableType.GetProperties();

            var enumeratedPrimaryKeyProperties =
                properties.Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            var primaryKeyProperties = enumeratedPrimaryKeyProperties as PropertyInfo[]
                                       ?? enumeratedPrimaryKeyProperties.ToArray();

            if (primaryKeyProperties.Length > 1)
            {
                throw new Exception("More than one primary key");
            }

            var primaryKeyProperty = primaryKeyProperties.Any()
                ? primaryKeyProperties.Single()
                : properties.Single(p => p.Name.ToLower() == "id");

            if (primaryKeyProperty.PropertyType != typeof(int))
            {
                throw new Exception("Primary key property must be an int");
            }

            return primaryKeyProperty;
        }
    }
}

