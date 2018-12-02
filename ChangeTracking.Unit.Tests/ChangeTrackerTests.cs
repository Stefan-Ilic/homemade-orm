using System;
using System.Linq;
using ChangerTracking.Interfaces;
using ChangeTracking.Entities;
using Shouldly;
using Xunit;

namespace ChangeTracking.Unit.Tests
{
    public class ChangeTrackerTests
    {
        [Fact]
        public void AddEntry_HasItem_ContainsEntry()
        {
            var myObject = new object();
            var entry = new ChangeTrackerEntry
            {
                Item = myObject
            };
            IChangeTracker changeTracker = new ChangeTracker();

            changeTracker.AddEntry(entry);

            changeTracker.Count.ShouldBe(1);
            changeTracker.Entries.Single().Item.ShouldBe(myObject);
        }

        [Fact]
        public void AddEntry_NoItem_ThrowsException()
        {
            var entry = new ChangeTrackerEntry();
            IChangeTracker changeTracker = new ChangeTracker();

            Should.Throw<Exception>(() => changeTracker.AddEntry(entry));
        }

        [Fact]
        public void GetEntryByObject_NoSuchEntry_ThrowsException()
        {
            IChangeTracker changeTracker = new ChangeTracker();

            Should.Throw<Exception>(() => changeTracker.GetEntry(new object()));
        }

        [Fact]
        public void GetEntryByIdAndType_NoSuchEntry_ThrowsException()
        {
            IChangeTracker changeTracker = new ChangeTracker();

            Should.Throw<Exception>(() => changeTracker.GetEntry(1, typeof(int)));
        }

        [Fact]
        public void GetEntryByObject_EntryExists_CorrectEntry()
        {
            var myObject = new object();
            var entry = new ChangeTrackerEntry
            {
                Item = myObject
            };
            IChangeTracker changeTracker = new ChangeTracker();
            changeTracker.AddEntry(entry);

            var retrievedEntry = changeTracker.GetEntry(myObject);

            retrievedEntry.ShouldBe(entry);
            retrievedEntry.Item.ShouldBe(myObject);
        }

        [Fact]
        public void InsertedObjects_HaveBeenAdded_ReturnEntries()
        {
            var myObject = new object();
            var entry = new ChangeTrackerEntry
            {
                Item = myObject,
                State = ChangeTrackerEntry.States.Inserted
            };
            IChangeTracker changeTracker = new ChangeTracker();
            changeTracker.AddEntry(entry);

            changeTracker.InsertedObjects.Single().ShouldBe(myObject);
        }

        [Fact]
        public void ModifiedObjects_HaveBeenAdded_ReturnEntries()
        {
            var myObject = new object();
            var entry = new ChangeTrackerEntry
            {
                Item = myObject,
                State = ChangeTrackerEntry.States.Modified
            };
            IChangeTracker changeTracker = new ChangeTracker();
            changeTracker.AddEntry(entry);

            changeTracker.ModifiedObjects.Single().ShouldBe(myObject);
        }

        [Fact]
        public void DeletedObjects_HaveBeenAdded_ReturnEntries()
        {
            var myObject = new object();
            var entry = new ChangeTrackerEntry
            {
                Item = myObject,
                State = ChangeTrackerEntry.States.Deleted
            };
            IChangeTracker changeTracker = new ChangeTracker();
            changeTracker.AddEntry(entry);

            changeTracker.DeletedObjects.Single().ShouldBe(myObject);
        }

        [Fact]
        public void UnmodifiedObjects_HaveBeenAdded_ReturnEntries()
        {
            var myObject = new object();
            var entry = new ChangeTrackerEntry
            {
                Item = myObject,
                State = ChangeTrackerEntry.States.Unmodified
            };
            IChangeTracker changeTracker = new ChangeTracker();
            changeTracker.AddEntry(entry);

            changeTracker.UnmodifiedObjects.Single().ShouldBe(myObject);
        }
    }
}
