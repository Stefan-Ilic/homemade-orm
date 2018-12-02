using System;
using System.Collections.Generic;
using System.Linq;
using ChangerTracking.Interfaces;
using ChangeTracking.Entities;
using DatabaseDriver.Interfaces;
using Moq;
using Shouldly;
using SqlStatementBuilder.Interfaces;
using Xunit;

namespace ORM.Unit.Tests
{
    public class MyOrmTests
    {
        [Fact]
        public void Insert_SetsId_NegativeOne()
        {
            var person = new Person();
            var mockDriver = new Mock<IDatabaseDriver>();
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.Insert(person);

            person.Id.ShouldBe(-1);
        }

        [Fact]
        public void Insert_AddsChangeTrackerEntry_Once()
        {
            var person = new Person();
            var mockDriver = new Mock<IDatabaseDriver>();
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.Insert(person);

            mockChangeTracker.Verify(x => x.AddEntry(It.IsAny<ChangeTrackerEntry>()), Times.Once);
        }

        [Fact]
        public void Delete_GetsChangeTrackerEntry_SetsDeleted()
        {
            var person = new Person();
            var entry = new ChangeTrackerEntry
            {
                Item = person,
                State = ChangeTrackerEntry.States.Unmodified
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(person)).Returns(entry);
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.Delete(person);

            mockChangeTracker.Verify(x => x.GetEntry(person), Times.Once);
            entry.State.ShouldBe(ChangeTrackerEntry.States.Deleted);
        }

        [Fact]
        public void SubmitChanges_PersonInserted_InvokesDriverRunInsertStatement()
        {
            var person = new Person();
            var entry = new ChangeTrackerEntry
            {
                Item = person,
                State = ChangeTrackerEntry.States.Inserted
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(person)).Returns(entry);
            mockChangeTracker.Setup(x => x.InsertedObjects).Returns(new List<object> {person});
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.SubmitChanges();

            mockDriver.Verify(x => x.RunInsertStatement(It.IsAny<string>()), Times.AtLeastOnce());
        }

        [Fact]
        public void SubmitChanges_PersonInserted_SetsStateUnmodified()
        {
            var person = new Person();
            var entry = new ChangeTrackerEntry
            {
                Item = person,
                State = ChangeTrackerEntry.States.Inserted
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(person)).Returns(entry);
            mockChangeTracker.Setup(x => x.InsertedObjects).Returns(new List<object> { person });
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.SubmitChanges();

            entry.State.ShouldBe(ChangeTrackerEntry.States.Unmodified);
        }

        [Fact]
        public void SubmitChanges_PersonInserted_SetsNewId()
        {
            var person = new Person { Id = -1 };
            var entry = new ChangeTrackerEntry
            {
                Item = person,
                State = ChangeTrackerEntry.States.Inserted
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            mockDriver.Setup(x => x.RunInsertStatement(It.IsAny<string>())).Returns(1);
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(person)).Returns(entry);
            mockChangeTracker.Setup(x => x.InsertedObjects).Returns(new List<object> { person });
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.SubmitChanges();

            person.Id.ShouldBe(1);
        }

        [Fact]
        public void SubmitChanges_PersonModified_SetsStateUnmodified()
        {
            var person = new Person();
            var entry = new ChangeTrackerEntry
            {
                Item = person,
                State = ChangeTrackerEntry.States.Modified
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(person)).Returns(entry);
            mockChangeTracker.Setup(x => x.ModifiedObjects).Returns(new List<object> { person });
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.SubmitChanges();

            entry.State.ShouldBe(ChangeTrackerEntry.States.Unmodified);
        }

        [Fact]
        public void SubmitChanges_PersonModified_InvokesDriverRunUpdateStatement()
        {
            var person = new Person();
            var entry = new ChangeTrackerEntry
            {
                Item = person,
                State = ChangeTrackerEntry.States.Modified
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(person)).Returns(entry);
            mockChangeTracker.Setup(x => x.ModifiedObjects).Returns(new List<object> { person });
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.SubmitChanges();

            mockDriver.Verify(x => x.RunUpdateStatement(It.IsAny<string>()), Times.AtLeastOnce());
        }

        [Fact]
        public void GetObjectsFromDb_InvokesDriverRunSelectStatement()
        {
            var mockDriver = new Mock<IDatabaseDriver>();
            mockDriver.Setup(x => x.RunSelectStatement(It.IsAny<string>()))
                .Returns(new List<IDictionary<string, object>>());
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            orm.GetObjectsFromDb<object>(mockSqlBuilder.Object);

            mockDriver.Verify(x => x.RunSelectStatement(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetObjectsFromDb_ObjectExists_UsesSameObject()
        {
            var id = new Random().Next(1, int.MaxValue);
            var person = new Person
            {
                Id = id,
                FirstName = "Mike",
                LastName = "Rosoft",
                Age = 1337
            };
            var entry = new ChangeTrackerEntry
            {
                Item = person,
                State = ChangeTrackerEntry.States.Unmodified
            };
            var props = new List<IDictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"Id", id},
                    {"FirstName", "Mike"},
                    {"LastName", "Rosoft"},
                    {"Age", 1337}
                }
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            mockDriver.Setup(x => x.RunSelectStatement(It.IsAny<string>()))
                .Returns(props);
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(id, typeof(Person))).Returns(entry);
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            var result = orm.GetObjectsFromDb<Person>(mockSqlBuilder.Object);

            result.Single().ShouldBe(person);
        }

        [Fact]
        public void GetObjectsFromDb_Materializes_Correctly()
        {
            var props = new List<IDictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"Id", 1},
                    {"FirstName", "Mike"},
                    {"LastName", "Rosoft"},
                    {"Age", 1337}
                }
            };
            var mockDriver = new Mock<IDatabaseDriver>();
            mockDriver.Setup(x => x.RunSelectStatement(It.IsAny<string>()))
                .Returns(props);
            var mockSqlBuilder = new Mock<ISqlStatementBuilder>();
            var mockChangeTracker = new Mock<IChangeTracker>();
            mockChangeTracker.Setup(x => x.GetEntry(It.IsAny<int>(), typeof(Person)))
                .Returns((ChangeTrackerEntry)null);
            var orm = new MyOrm(mockDriver.Object, mockSqlBuilder.Object, mockChangeTracker.Object);

            var result = orm.GetObjectsFromDb<Person>(mockSqlBuilder.Object);

            var person = result.Single().ShouldBeOfType<Person>();
            person.Id.ShouldBe(1);
            person.FirstName.ShouldBe("Mike");
            person.LastName.ShouldBe("Rosoft");
            person.Age.ShouldBe(1337);
        }
    }
}
