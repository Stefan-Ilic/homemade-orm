using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    public class ChangeTracker
    {
        public Dictionary<object, ChangeTrackerEntry> Entries { get; set; } = new Dictionary<object, ChangeTrackerEntry>();
        public Dictionary<(int, Type), ChangeTrackerEntry> EntriesWithId { get; set; } = new Dictionary<(int, Type), ChangeTrackerEntry>();
    }
}
