using System;
using System.Linq;
using ChangerTracking.Interfaces;
using DatabaseDriver;
using ORM;
using SqlStatementBuilder;

namespace RunnableProjectForTesting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sqlBuilder = new MySqlStatementBuilder();
            var driver = new MySqlDriver("");
            IChangeTracker changeTracker = new ChangeTracking.ChangeTracker();
            var orm = new MyOrm(driver, sqlBuilder, changeTracker);
            var qry = orm.GetQuery<Person>();

            var filtered = qry
                .Where(i => i.Age > 18 || i.Age < 30)
                .Where(i => i.FirstName == "Peter")
                .Where(i => i.FirstName != "Otto" && i.Age <= 100);

            //var filtered = qry.OrderBy(i => i.Age);

            var lst = filtered.ToList();

            //foreach (var person1 in lst)
            //{
            //    person1.Age++;
            //}

            //orm.SubmitChanges();
            
      

            //var person = new Person
            //{
            //    Age = 18,
            //    FirstName = "Manfred",
            //    LastName = "Fredmann"
            //};

            // orm.Insert(person);

            Console.ReadKey();
        }
    }
}
