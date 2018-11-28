using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ORM;
using Shouldly;
using Xunit;

namespace IntegrationTests
{
    internal class MysqlContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public MysqlContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class MySqlIntegrationTests
    {
        public MySqlIntegrationTests()
        {
            Docker.Start("orm-test-mysql");
        }


        [Fact]
        public void Insert()
        {
            const string validConnectionString = "server=127.0.0.1;uid=root;pwd=root;database=test";

            var orm = new MyOrm(validConnectionString);

            var options = new DbContextOptionsBuilder<MysqlContext>()
                .UseMySQL(validConnectionString)
                .Options;
            var context = new MysqlContext(options);

            orm.ChangeTracker.Entries.ShouldBeEmpty();
            orm.ChangeTracker.EntriesWithId.ShouldBeEmpty();

            var person = new Person
            {
                FirstName = "Mike",
                LastName = "Rosoft",
                Age = 1337
            };

            orm.Insert(person);

            orm.ChangeTracker.Entries.ShouldNotBeEmpty();
            orm.ChangeTracker.EntriesWithId.ShouldNotBeEmpty();
            orm.ChangeTracker.Entries.ShouldAllBe(x => x.Value.State == ChangeTrackerEntry.States.Inserted);

            context.Persons.ShouldBeEmpty();

            orm.SubmitChanges();

            context.Persons.ShouldNotBeEmpty();

        }
    }
}
