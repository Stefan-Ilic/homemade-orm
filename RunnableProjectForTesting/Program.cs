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
                .Where(i => i.FirstName == "Peter");

            var lst = filtered.ToList();

            foreach (var i in lst)
            {
                Console.WriteLine(i);
            }

            Console.ReadKey();
        }
    }
}
