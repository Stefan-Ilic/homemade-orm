using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    /// <summary>
    /// Tracks changed of CLR objects known to the ORM
    /// </summary>
    public class ChangeTracker
    {
        /// <summary>
        /// Holds ChangeTrackerEntries and the objects that belong to them
        /// </summary>
        public Dictionary<object, ChangeTrackerEntry> Entries { get; set; } = new Dictionary<object, ChangeTrackerEntry>();

        /// <summary>
        /// Holds ChangeTrackerEntries and the ids and types that belong to them
        /// </summary>
        public Dictionary<(int, Type), ChangeTrackerEntry> EntriesWithId { get; set; } = new Dictionary<(int, Type), ChangeTrackerEntry>();
    }
}
