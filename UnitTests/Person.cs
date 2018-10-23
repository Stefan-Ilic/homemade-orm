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

        [Column("firstname")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column("4g3")]
        public int Age { get; set; }
    }
}
