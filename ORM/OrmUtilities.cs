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
        public static Dictionary<string, Type> GetColumnNamesAndTypes(Type tableType)
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

        //TODO  duplicated code, put in own method
        public static Dictionary<string, object> GetColumnNamesAndValues(object objectToInsert)
        {
            var list = new Dictionary<string, object>();
            var properties = objectToInsert.GetType().GetProperties();

            if (properties == null) return list;

            foreach (var property in properties)
            {
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                list.Add(columnAttribute != null ? columnAttribute.ColumnName : property.Name,
                    property.GetValue(objectToInsert));
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
