﻿using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudHooks
{
    /// <summary>
    /// A CrudHook that will only be executed after the given model was deleted.
    /// The model's navigation properties will be null.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudHook.</typeparam>
    abstract class DeleteHook<T> : CrudHook<T> where T : Model
    {
        protected DeleteHook(T model, CMDDatabaseContext context)
            : base(model, context)
        {
        }
    }
}
