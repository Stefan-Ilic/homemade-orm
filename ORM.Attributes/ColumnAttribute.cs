using System;

namespace ORM.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Provides the ability to change the database column name of a property
    /// </summary>
    public class ColumnAttribute : Attribute
    {
        /// <inheritdoc />
        public ColumnAttribute(string name = "")
        {
            ColumnName = name;
        }

        /// <summary>
        /// The column name that is displayed in the database
        /// </summary>
        public string ColumnName { get; set; }
    }
}
