using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.Attributes
{
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name = "")
        {
            ColumnName = name;
        }

        public string ColumnName { get; set; }
    }
}
