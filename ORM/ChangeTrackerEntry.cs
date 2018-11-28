using System;
using System.Collections.Generic;
using System.Reflection;

namespace ORM
{
    public class ChangeTrackerEntry
    {
        public object Item { get; set; }

        public Dictionary<PropertyInfo, object> Originals { get; set; }

        public States State { get; set; }

        public enum States
        {
            Unmodified,
            Modified,
            Inserted,
            Deleted
        }
    }
}