using System;
using System.Collections.Generic;
using System.Text;
using ORM.Attributes;

namespace UnitTests
{
    [Table("people")]
    internal class Person
    {
        public int Id { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column("Age")]
        public int Age { get; set; }
    }
}
