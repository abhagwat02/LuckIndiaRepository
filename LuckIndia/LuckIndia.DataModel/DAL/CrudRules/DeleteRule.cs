
using LuckIndia.DataModel;
using LuckIndia.DataModel.DAL.CrudRules;
using LuckIndia.Models;

namespace LuckIndia.DataModel.DAL.CrudRules

{
    /// <summary>
    /// A CrudRule that will only be executed after the given model was deleted.
    /// The model's navigation properties will be null.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudRule.</typeparam>
    abstract class DeleteRule<T> : CrudRule<T> where T : Model
    {
        protected DeleteRule(T model, LuckIndiaDBContext context)
            : base(model, context)
        {
        }
    }
}
