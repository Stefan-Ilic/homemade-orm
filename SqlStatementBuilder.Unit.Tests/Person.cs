using System.ComponentModel.DataAnnotations.Schema;

namespace SqlStatementBuilder.Unit.Tests
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
