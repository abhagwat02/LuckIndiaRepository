using LuckIndia.DataModel.Interfaces;
using LuckIndia.Models;

namespace LuckIndia.DataModel.DAL.CrudRules
{
    /// <summary>
    /// Rules that will be executed during CRUD operations.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudRule.</typeparam>
    abstract class CrudRule<T> : IExecutable where T : Model
    {
        protected CrudRule(T model, LuckIndiaDBContext context)
        {
            this.Model = model;
            this.Context = context;
        }

        protected T Model { get; private set; }

        protected LuckIndiaDBContext Context { get; private set; }

        /// <summary>
        /// A higher priority will be executed before any other CrudRules with a lower priority.
        /// Defaut is int.MinValue.
        /// </summary>
        public virtual int Priority
        {
            get
            {
                return int.MinValue;
            }
        }

        public abstract void Execute();
    }
}
