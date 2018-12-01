using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.Attributes
{
    /// <summary>
    /// Used to describe a class used as a table
    /// </summary>
    public class TableAttribute : Attribute
    {
        /// <inheritdoc />
        public TableAttribute(string name = "")
        {
            TableName = name;
        }

        /// <summary>
        /// Used to change the default table name from class name to something else
        /// </summary>
        public string TableName { get; set; }   
    }
}
