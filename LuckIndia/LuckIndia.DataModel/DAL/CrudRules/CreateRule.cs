using LuckIndia.Models;

namespace LuckIndia.DataModel.DAL.CrudRules
{
    /// <summary>
    /// A CrudRule that will only be executed after the given model was created.
    /// The model's navigation properties will be null.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudRule.</typeparam>
    abstract class CreateRule<T> : CrudRule<T> where T : Model
    {
        protected CreateRule(T model, LuckIndiaDBContext context)
            : base(model, context)
        {
        }
    }
}
