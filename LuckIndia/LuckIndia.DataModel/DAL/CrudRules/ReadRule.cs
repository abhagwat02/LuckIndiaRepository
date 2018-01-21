using LuckIndia.Models;

namespace LuckIndia.DataModel.DAL.CrudRules
{
    /// <summary>
    /// A CrudRule that will only be executed after the given model was read.
    /// The model's navigation properties will be available.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudRule.</typeparam>
    abstract class ReadRule<T> : CrudRule<T> where T : Model
    {
        protected ReadRule(T model, LuckIndiaDBContext context)
            : base(model, context)
        {
        }
    }
}
