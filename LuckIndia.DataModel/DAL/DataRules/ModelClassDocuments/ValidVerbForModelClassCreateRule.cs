using System;
using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ModelClassDocuments
{
    sealed class ValidVerbForModelClassCreateRule : CreateRule<ModelClassDocument>
    {
        private static readonly HashSet<String> VerbSet = new HashSet<string> { "GET", "POST" };

        public ValidVerbForModelClassCreateRule(ModelClassDocument model) : base(model)
        {
        }

        public override void Validate()
        {
            if (!VerbSet.Contains(this.Model.Verb))
            {
                throw new InvalidDataException("This is not a valid verb for modelclass documentation. Please use one of the following: GET POST");
            }
        }
    }
}
