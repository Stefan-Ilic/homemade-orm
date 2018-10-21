using System;
using System.Collections.Generic;
using System.Text;
using ORM;
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
            var person = new Person
            {
                FirstName = "Mike",
                LastName = "Rosoft",
                Age = 1337
            };

            orm.Insert(person);

        }
    }
}
