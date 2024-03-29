﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ORM.Attributes;

namespace ORM.Utilities
{
    /// <summary>
    /// Provides useful static functions used by multiple classes in the framework
    /// </summary>
    public abstract class OrmUtilities
    {
        /// <summary>
        /// Returns columns in a format expected by ISqlStatementbuilder
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns columns in a format expected by ISqlStatementbuilder
        /// </summary>
        /// <param name="objectToInsert"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the class name of a CLR type, or the table name if table attribute is set
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public static string GetTableName(Type tableType)
        {
            var className = tableType.Name;
            var tableAttributeName = tableType.GetCustomAttribute<TableAttribute>()?.TableName;
            return tableAttributeName ?? className;
        }

        /// <summary>
        /// Returns the property info of the property marked is primary key
        /// If none is marked, returns the property name id
        /// Has to be an int
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public static PropertyInfo GetPrimaryKeyProperty(Type tableType)
        {
            var properties = tableType.GetProperties();

            var primaryKeyProperties =
                properties.Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToArray();

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

