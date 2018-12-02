using ORM.Attributes;

namespace RunnableProjectForTesting
{
    public class Person
    {
        [Column("firstname")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column("4g3")]
        public int Age { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {Age}";
        }
    }
}
