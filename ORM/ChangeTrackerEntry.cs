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

        public void UpdateOriginals(object objectToUpdate)
        {
            if (objectToUpdate != Item)
            {
                throw new Exception("The passed object is not the stored object");
            }

            var properties = objectToUpdate.GetType().GetProperties();

            Originals = new Dictionary<PropertyInfo, object>();
            foreach (var property in properties)
            {
               Originals.Add(property, property.GetValue(objectToUpdate));
            }
        }
    }
}