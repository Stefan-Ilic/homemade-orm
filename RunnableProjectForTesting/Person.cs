using System;
using System.Collections.Generic;
using System.Text;
using ORM.Attributes;

namespace RunnableProjectForTesting
{
    [Table("persons")]
    public class Person
    {
        [Column("firstname")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {Age}";
        }
    }
}
