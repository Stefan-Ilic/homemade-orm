using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.Attributes
{
    public class TableAttribute : Attribute
    {
        public TableAttribute(string name = "")
        {
            TableName = name;
        }

        public string TableName { get; set; }   
    }
}
