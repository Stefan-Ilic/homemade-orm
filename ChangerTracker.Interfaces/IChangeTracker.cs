using System;
using System.Collections.Generic;
using ChangeTracking.Entities;

namespace ChangerTracking.Interfaces
{
    /// <summary>
    /// Tracks changes of objects known to the ORM
    /// </summary>
    public interface IChangeTracker
    {
        /// <summary>
        /// Returns the entry that belongs to given object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ChangeTrackerEntry GetEntry(object obj);

        /// <summary>
        /// Returns the entry that belongs to an object of a certain type with a certain id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ChangeTrackerEntry GetEntry(int id, Type type);

        /// <summary>
        /// Returns all Entries
        /// </summary>
        IEnumerable<ChangeTrackerEntry> Entries { get; }

        /// <summary>
        /// Adds an entry to the change tracker
        /// </summary>
        /// <param name="entry"></param>
        void AddEntry(ChangeTrackerEntry entry);

        /// <summary>
        /// The number of entries in the change tracker
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns all objects marked as inserted
        /// </summary>
        IEnumerable<object> InsertedObjects { get; }

        /// <summary>
        /// Returns all objects marked as unmodified
        /// </summary>
        IEnumerable<object> UnmodifiedObjects { get; }

        /// <summary>
        /// Returns all objects marked as modified
        /// </summary>
        IEnumerable<object> ModifiedObjects { get; }

        /// <summary>
        /// Returns all objects marked as deleted
        /// </summary>
        IEnumerable<object> DeletedObjects { get; }
    }
}
