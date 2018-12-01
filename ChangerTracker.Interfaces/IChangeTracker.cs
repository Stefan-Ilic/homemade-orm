using System;
using System.Collections.Generic;
using System.Text;
using ChangeTracker.Entities;

namespace ChangerTracker.Interfaces
{
    public interface IChangeTracker
    {
        ChangeTrackerEntry GetEntry(object obj);
        ChangeTrackerEntry GetEntry(int id, Type type);
        IEnumerable<ChangeTrackerEntry> Entries { get; }
        void AddEntry(ChangeTrackerEntry entry);
        int Count { get; }
        IEnumerable<object> InsertedObjects { get; }
        IEnumerable<object> UnmodifiedObjects { get; }
        IEnumerable<object> ModifiedObjects { get; }
        IEnumerable<object> DeletedObjects { get; }
    }
}
