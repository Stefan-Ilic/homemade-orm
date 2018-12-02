using System;
using System.Collections.Generic;
using System.Reflection;

namespace ChangeTracking.Entities
{
    /// <summary>
    /// Represents an Entry in the ORMs change tracker 
    /// </summary>
    public class ChangeTrackerEntry
    {
        /// <summary>
        /// The object being tracked
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// The original values of the object's properties before it was changed
        /// </summary>
        public Dictionary<PropertyInfo, object> Originals { get; set; }

        /// <summary>
        /// The current state of the entry
        /// </summary>
        public States State { get; set; }

        /// <summary>
        /// Describes the current state of the object
        /// </summary>
        public enum States
        {
            /// <summary>
            /// The object is thought to be unmodified
            /// It is still possible that it has been modified
            /// </summary>
            Unmodified,

            /// <summary>
            /// The object has definitely been modified
            /// </summary>
            Modified,

            /// <summary>
            /// The object has been inserted into the ORM
            /// </summary>
            Inserted,

            /// <summary>
            /// The object has been deleted from the orm
            /// </summary>
            Deleted
        }

        /// <summary>
        /// Updates the original values of the object's properties
        /// after the changes are submitted 
        /// </summary>
        /// <param name="objectToUpdate"></param>
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