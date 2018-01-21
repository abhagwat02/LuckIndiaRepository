using LuckIndia.DataModel.DAL.CachingServices;
using LuckIndia.DataModel.Interfaces;
using LuckIndia.Models;
using System.Collections.Generic;
using System.Linq;

namespace LuckIndia.DataModel.DAL.CrudHelpers
{
    sealed class ReadHelper<T> : CrudHelper<T> where T : Model
    {
        private readonly int _id;
        private T _model;

        public ReadHelper(int id, LuckIndiaDBContext context)
            : base(null, context)
        {
            _id = id;
        }

        protected override void ValidateApplication()
        {
            //no need to validate anything because the collection will validate the application.
        }

        protected override void ValidateUser()
        {
            //no need to validate anything because the collection will validate the user.
        }

        protected override IEnumerable<IExecutable> GetCrudRules()
        {
            return CrudRulesCachingService.GetCurrent().GetReadRules(this.Model, this.Context);
        }

        //protected override IEnumerable<CrudHook<T>> GetCrudHooks()
        //{
        //    return CrudHooksCachingService.GetCurrent().GetReadHooks(this.Model, this.Context);
        //}


        protected override void ValidateCollection()
        {
            _model = this.Context.GetCollection<T>().FirstOrDefault(x => x.Id == _id);
            if (_model == null)
            {
                //throw new ModelNotFoundException();
            }
        }

        /// <summary>
        /// This will get the requested model. This can only be called after Execute has ran or it will be null.
        /// </summary>
        public T GetModel()
        {
            return _model;
        }
    }
}
