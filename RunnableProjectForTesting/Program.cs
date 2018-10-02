using System;
using System.Linq;
using ORM;

namespace RunnableProjectForTesting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var qry = new QueryableObject<Person>();

            var filtered = qry
                .Where(i => i.Age > 18 || i.Age < 30)
                .Where(i => i.FirstName == "Peter")
                .Where(i => i.FirstName != "Otto" || i.Age <= 100);

            var lst = filtered.ToList();

            Console.ReadKey();
        }
    }
}
