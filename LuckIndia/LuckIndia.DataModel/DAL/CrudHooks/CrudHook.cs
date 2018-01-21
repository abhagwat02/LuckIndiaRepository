using LuckIndia.DataModel.Interfaces;
using LuckIndia.Models;

namespace LuckIndia.DataModel.DAL.CrudHooks
{
    /// <summary>
    /// CrudHooks that will be executed during CRUD operations.
    /// CrudHooks are only executed if the current application is subscribed to the CrudHook via ApplicationCrudHooks.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudHook.</typeparam>
    abstract class CrudHook<T> : IExecutable where T : Model
    {
        protected CrudHook(T model, LuckIndiaDBContext context)
        {
            this.Model = model;
            this.Context = context;
        }

        protected T Model { get; private set; }

        protected LuckIndiaDBContext Context { get; private set; }

        /// <summary>
        /// This value must match the CrudHook.Id from the context.
        /// </summary>
       // public abstract CrudHookValue CrudHookValue { get; }

        /// <summary>
        /// A higher priority will be executed before any other CrudHooks with a lower priority.
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
