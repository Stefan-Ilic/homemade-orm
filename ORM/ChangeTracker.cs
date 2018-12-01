using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM
{
    /// <summary>
    /// Tracks changed of CLR objects known to the ORM
    /// </summary>
    public class ChangeTracker
    {

        private readonly Dictionary<object, ChangeTrackerEntry> _entries =
            new Dictionary<object, ChangeTrackerEntry>();

        /// <summary>
        /// Returns ChangeTrackerEntry with of object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ChangeTrackerEntry GetEntry(object obj)
        {
            return _entries[obj];
        }

        /// <summary>
        /// Returns ChangeTrackerEntry with id and type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public ChangeTrackerEntry GetEntry(int id, Type type)
        {
            var idProperty = OrmUtilities.GetPrimaryKeyProperty(type);
            return _entries.Values.
                FirstOrDefault(entry => entry.Item.GetType() == type 
                                && idProperty.GetValue(entry.Item).Equals(id));
        }

        /// <summary>
        /// Returns all change tracker entries
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ChangeTrackerEntry> GetAllEntries()
        {
            return _entries.Values;
        }

        /// <summary>
        /// Adds an entry to the change tracker
        /// </summary>
        /// <param name="entry"></param>
        public void AddEntry(ChangeTrackerEntry entry)
        {
            if (entry.Item == null)
            {
                throw new Exception("Item is null");
            }

            _entries.Add(entry.Item, entry);
        }

        /// <summary>
        /// The number of entries tracked
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Get all entries with State inserted
        /// </summary>
        public IEnumerable<object> InsertedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Inserted)
                .Select(x => x.Key);

        /// <summary>
        /// Get all entries with State unmodified
        /// </summary>
        public IEnumerable<object> UnmodifiedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Unmodified)
                .Select(x => x.Key);

        /// <summary>
        /// Get all entries with State deleted
        /// </summary>
        public IEnumerable<object> DeletedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Deleted)
                .Select(x => x.Key);

        /// <summary>
        /// Get all entries with State modified
        /// </summary>
        public IEnumerable<object> ModifiedObjects =>
            _entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Modified)
                .Select(x => x.Key);
    }
}
