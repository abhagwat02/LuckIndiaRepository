using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace LuckIndia.Models
{
    //I wanted this to be an interface, but because we need a concrete class to use the DbSet's in EF the poco needs to be newable and typeof needs to return a class, not an interface.

    public abstract class Model : IComparable
    {

        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();


        public abstract int Id { get; set; }

        public int CompareTo(object obj)
        {
            var model = obj as Model;

            if (model == null)
            {
                return -1;
            }

            var sortedModel = model as ISortable;
            var sortedThis = this as ISortable;

            if (sortedModel == null || sortedThis == null)
            {
                return this.Id.CompareTo(model.Id);
            }

            return sortedThis.Sort.CompareTo(sortedModel.Sort);

        }
        

        protected T GetPropertyValue<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            object value;

            if (_propertyBackingDictionary.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }


        protected bool SetPropertyValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            if (EqualityComparer<T>.Default.Equals(newValue, GetPropertyValue<T>(propertyName)))
            {
                return false;
            }

            _propertyBackingDictionary[propertyName] = newValue;

            return true;
        }
    }
}
