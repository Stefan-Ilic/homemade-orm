using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IntegrationTests
{
    [ORM.Attributes.Table("people")]
    [Table("people")]
    internal class Person
    {
        public int Id { get; set; }

        [ORM.Attributes.Column("FirstName")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [ORM.Attributes.Column("Age")]
        public int Age { get; set; }
    }
}
