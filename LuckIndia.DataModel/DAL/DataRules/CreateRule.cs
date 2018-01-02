﻿using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules
{
    /// <summary>
    /// A DataRule that will only be applied when the given model is going to be created.
    /// The model's Id will always be zero and all navigation properties will be null.
    /// </summary>
    /// <typeparam name="T">Type of Model being validated.</typeparam>
    abstract class CreateRule<T> : DataRule<T> where T : Model
    {
        protected CreateRule(T model)
            : base(model)
        {
        }
    }
}
