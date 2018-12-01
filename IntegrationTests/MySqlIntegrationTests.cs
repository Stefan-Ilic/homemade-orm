﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseDriver;
using Microsoft.EntityFrameworkCore;
using ORM;
using Shouldly;
using SqlStatementBuilder;
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
        public void Insert_Update_Delete_Select()
        {
            const string validConnectionString = "server=127.0.0.1;uid=root;pwd=root;database=test";
            var options = new DbContextOptionsBuilder<MysqlContext>()
                .UseMySQL(validConnectionString)
                .Options;
            var context = new MysqlContext(options);
            context.RemoveRange(context.Persons);
            context.SaveChanges();

            var driver = new MySqlDriver(validConnectionString);
            var sqlBuilder = new MySqlStatementBuilder();
            var orm = new MyOrm(driver, sqlBuilder);

            orm.ChangeTracker.Entries.ShouldBeEmpty();

            var person1 = new Person
            {
                FirstName = "Mike",
                LastName = "Rosoft",
                Age = 1337
            };

            var person2 = new Person
            {
                FirstName = "Tux",
                LastName = "L. Oves",
                Age = 80085
            };

            var person3 = new Person
            {
                FirstName = "Mac",
                LastName = "N. Tosh",
                Age = 1234
            };

            //Insert
            person1.Id.ShouldBe(0);
            orm.Insert(person1);
            person1.Id.ShouldBe(-1);

            person2.Id.ShouldBe(0);
            orm.Insert(person2);
            person2.Id.ShouldBe(-2);

            person3.Id.ShouldBe(0);
            orm.Insert(person3);
            person3.Id.ShouldBe(-3);

            orm.ChangeTracker.Entries.Count.ShouldBe(3);
            orm.ChangeTracker.Entries.ShouldAllBe(x => x.Value.State == ChangeTrackerEntry.States.Inserted);

            GetFreshContext(options).Persons.ShouldBeEmpty();

            orm.SubmitChanges();
            person1.Id.ShouldBeGreaterThan(0);
            person2.Id.ShouldBe(person1.Id + 1);
            person3.Id.ShouldBe(person2.Id + 1);

            GetFreshContext(options).Persons.Count().ShouldBe(3);
            orm.ChangeTracker.Entries.ShouldAllBe(x => x.Value.State == ChangeTrackerEntry.States.Unmodified);

            //Update
            var rand = new Random();
            var newAge = rand.Next(1, int.MaxValue);
            person1.Age = newAge;

            GetFreshContext(options).Persons.Single(x => x.Id == person1.Id).Age.ShouldBe(1337);
            orm.SubmitChanges();
            orm.ChangeTracker.Entries.Count(x => x.Value.State == ChangeTrackerEntry.States.Unmodified).ShouldBe(3);
            GetFreshContext(options).Persons.Single(x => x.Id == person1.Id).Age.ShouldBe(newAge);
            GetFreshContext(options).Persons.Count().ShouldBe(3);

            //Delete
            GetFreshContext(options).Persons.Count().ShouldBe(3);
            orm.ChangeTracker.Entries.ShouldAllBe(x => x.Value.State == ChangeTrackerEntry.States.Unmodified);

            orm.Delete(person3);
            orm.ChangeTracker.Entries.Count.ShouldBe(3);
            orm.ChangeTracker.Entries.Values.Count(x => x.State == ChangeTrackerEntry.States.Deleted).ShouldBe(1);
            orm.ChangeTracker.Entries[person3].State.ShouldBe(ChangeTrackerEntry.States.Deleted);
            GetFreshContext(options).Persons.Count().ShouldBe(3);

            orm.SubmitChanges();
            orm.ChangeTracker.Entries.Count.ShouldBe(3);
            orm.ChangeTracker.Entries.Values.Count(x => x.State == ChangeTrackerEntry.States.Deleted).ShouldBe(1);
            orm.ChangeTracker.Entries[person3].State.ShouldBe(ChangeTrackerEntry.States.Deleted);
            GetFreshContext(options).Persons.Count().ShouldBe(2);

            //Select
            var anotherPerson1 = orm.GetQuery<Person>()
                .Where(p => p.FirstName == "Mike" &&
                            p.LastName == "Rosoft")
                .Where(p => p.Age == newAge).ToList().First();

            anotherPerson1.ShouldNotBeNull();
            anotherPerson1.Id.ShouldBe(person1.Id);
            person1.ShouldBe(anotherPerson1);

            var person1AndPerson2 = orm.GetQuery<Person>()
                .Where(p => p.FirstName == "Mike" || p.FirstName == "Tux" &&
                            p.LastName == "Rosoft" || p.LastName == "L. Oves")
                .Where(p => p.Age == newAge || p.Age == 80085).ToList();

            person1AndPerson2[0].ShouldNotBeNull();
            person1AndPerson2[1].ShouldNotBeNull();

            person1AndPerson2[0].ShouldBe(anotherPerson1);
            person1AndPerson2[0].ShouldBe(person1);
            person1AndPerson2[1].ShouldBe(person2);
        }

        [Fact]
        public void Select_Multiple()
        {
            const string validConnectionString = "server=127.0.0.1;uid=root;pwd=root;database=test";
            var options = new DbContextOptionsBuilder<MysqlContext>()
                .UseMySQL(validConnectionString)
                .Options;
            var context = new MysqlContext(options);
            context.RemoveRange(context.Persons);
            context.SaveChanges();

            var driver = new MySqlDriver(validConnectionString);
            var sqlBuilder = new MySqlStatementBuilder();
            var orm = new MyOrm(driver, sqlBuilder);

            orm.ChangeTracker.Entries.ShouldBeEmpty();

            var person1 = new Person
            {
                FirstName = "Mike",
                LastName = "Rosoft",
                Age = 1337
            };

            var person2 = new Person
            {
                FirstName = "Tux",
                LastName = "L. Oves",
                Age = 80085
            };

            var person3 = new Person
            {
                FirstName = "Mac",
                LastName = "N. Tosh",
                Age = 1234
            };

            var inserterContext = GetFreshContext(options);
            inserterContext.Persons.Add(person1);
            inserterContext.Persons.Add(person2);
            inserterContext.Persons.Add(person3);
            inserterContext.SaveChanges();
            GetFreshContext(options).Persons.Count().ShouldBe(3);

            orm.ChangeTracker.Entries.ShouldBeEmpty();

            var people = orm.GetQuery<Person>().ToList();

            orm.ChangeTracker.Entries.Count.ShouldBe(3);
            orm.ChangeTracker.Entries[people[0]].ShouldNotBeNull();
            orm.ChangeTracker.Entries[people[1]].ShouldNotBeNull();
            orm.ChangeTracker.Entries[people[2]].ShouldNotBeNull();

            people[0].Id.ShouldBe(person1.Id);
            people[1].Id.ShouldBe(person2.Id);
            people[2].Id.ShouldBe(person3.Id);

            people[0].ShouldNotBe(person1);
            people[1].ShouldNotBe(person2);
            people[2].ShouldNotBe(person3);

            var evenMorePeople = orm.GetQuery<Person>().ToList();

            people[0].ShouldBe(evenMorePeople[0]);
            people[1].ShouldBe(evenMorePeople[1]);
            people[2].ShouldBe(evenMorePeople[2]);
        }

        private static MysqlContext GetFreshContext(DbContextOptions options)
        {
            return new MysqlContext(options);
        }
    }
}
