using LuckIndia.Models;
using LuckIndia.Models.Interfaces;

namespace LuckIndia.DataModel.DAL.DataRules
{
    /// <summary>
    /// Rules that will be validated during CRUD operations.
    /// Validate should not rely on a current user or application being available.
    /// </summary>
    /// <typeparam name="T">Type of Model being validated.</typeparam>
    abstract class DataRule<T> : IValidatable where T : Model
    {
        protected DataRule(T model)
        {
            this.Model = model;
        }

        protected T Model { get; private set; }

        public abstract void Validate();
    }
}
