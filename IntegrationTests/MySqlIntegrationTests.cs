using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ORM;
using Shouldly;
using Xunit;

namespace IntegrationTests
{
    public class MySqlIntegrationTests
    {
        public MySqlIntegrationTests()
        {
            Docker.Start("orm-test-mysql");
        }

        private const string ValidConnectionString = "server=127.0.0.1;uid=root;pwd=root;database=test";

        [Fact]
        public void Integration()
        {
            var orm = new MyOrm(ValidConnectionString);
            var random = new Random();
            var id = random.Next(1, 10000);
            var person = new Person
            {
                Id = id,
                FirstName = "Mike",
                LastName = "Rosoft",
                Age = 1337
            };

            orm.Insert(person);

            //var list = orm.GetQuery<Person>()
            //    .Where(x => x.FirstName == "Mike" 
            //                && x.LastName == "Rosoft" && x.Age == 1337).ToList();

            var list = orm.GetQuery<Person>().ToList();

            list.ShouldNotBeEmpty();
        }
    }
}
