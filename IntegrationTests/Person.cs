using System.ComponentModel.DataAnnotations.Schema;

namespace ORM.Integration.Tests
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
