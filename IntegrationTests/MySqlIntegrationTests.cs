using System;
using System.Collections.Generic;
using System.Text;
using ORM;
using Xunit;

namespace IntegrationTests
{
    public class MySqlIntegrationTests
    {
        private const string ValidConnectionString = "server=127.0.0.1;uid=root;pwd=root;database=test";

        [Fact]
        public void Integration()
        {
            var orm = new MyOrm(ValidConnectionString);
            orm.Connect();
        }
    }
}
