using System;
using System.Linq;
using ORM;

namespace RunnableProjectForTesting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var orm = new MyOrm();
            var qry = orm.GetQuery<Person>();

            var filtered = qry
                .Where(i => i.Age > 18 || i.Age < 30)
                .Where(i => i.FirstName == "Peter")
                .Where(i => i.FirstName != "Otto" && i.Age <= 100);

            //var filtered = qry.OrderBy(i => i.Age);

            var lst = filtered.ToList();

            Console.ReadKey();
        }
    }
}
