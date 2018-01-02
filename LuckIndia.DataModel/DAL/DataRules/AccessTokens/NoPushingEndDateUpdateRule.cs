using System;
using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.AccessTokens
{
    sealed class NoPushingEndDateUpdateRule : UpdateRule<AccessToken>
    {
        public NoPushingEndDateUpdateRule(AccessToken model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var endDateKey = DAL.PropertyName<AccessToken>(x => x.EndDate);

            if (!this.Delta.ContainsKey(endDateKey))
            {
                return;
            }

            var currentEndDate = this.Model.EndDate;

            var attemptedDate = this.Delta[endDateKey] as DateTime?;

            if (attemptedDate.HasValue && currentEndDate.HasValue && attemptedDate.Value > currentEndDate.Value)
            {
                throw new InvalidDataException("EndDate must be less than the current EndDate.");
            }
        }
    }
}
