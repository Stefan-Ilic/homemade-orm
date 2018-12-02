using System;
using System.Collections.Generic;
using System.Linq;
using ChangerTracking.Interfaces;
using ChangeTracking.Entities;
using ORM.Utilities;

namespace ChangeTracking
{
    /// <inheritdoc />
    public class ChangeTracker : IChangeTracker
    {

        private readonly Dictionary<object, ChangeTrackerEntry> _entries =
            new Dictionary<object, ChangeTrackerEntry>();


        /// <inheritdoc />
        public ChangeTrackerEntry GetEntry(object obj)
        {
            return _entries[obj];
        }

        /// <inheritdoc />
        public ChangeTrackerEntry GetEntry(int id, Type type)
        {
            var idProperty = OrmUtilities.GetPrimaryKeyProperty(type);
            return _entries.Values.
                FirstOrDefault(entry => entry.Item.GetType() == type 
                                && idProperty.GetValue(entry.Item).Equals(id));
        }

        /// <inheritdoc />
        public IEnumerable<ChangeTrackerEntry> Entries => _entries.Values;


        /// <inheritdoc />
        public void AddEntry(ChangeTrackerEntry entry)
        {
            if (entry.Item == null)
            {
                throw new Exception("Item is null");
            }
            _entries.Add(entry.Item, entry);
        }

        /// <inheritdoc />
        public int Count => _entries.Count;

        /// <inheritdoc />
        public IEnumerable<object> InsertedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Inserted)
                .Select(x => x.Key);

        /// <inheritdoc />
        public IEnumerable<object> UnmodifiedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Unmodified)
                .Select(x => x.Key);

        /// <inheritdoc />
        public IEnumerable<object> DeletedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Deleted)
                .Select(x => x.Key);

        /// <inheritdoc />
        public IEnumerable<object> ModifiedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Modified)
                .Select(x => x.Key);
    }
}
