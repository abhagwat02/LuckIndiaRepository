using System;
using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.UserPasswordRequests
{
    sealed class ValidEndDateUpdateRule : UpdateRule<UserPasswordRequest>
    {
        public ValidEndDateUpdateRule(UserPasswordRequest model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var endDateKey = DAL.PropertyName<UserPasswordRequest>(x => x.EndDate);
            if (!this.Delta.ContainsKey(endDateKey))
            {
                return;
            }

            if (this.Delta[endDateKey] == null)
            {
                throw new InvalidDataException("EndDate must have a value.");
            }

            var currentEndDate = this.Model.EndDate;
            DateTime attemptedDate;

            if (DateTime.TryParse(this.Delta[endDateKey].ToString(), out attemptedDate))
            {
                DateTime.SpecifyKind(attemptedDate, DateTimeKind.Utc);

                if (currentEndDate.HasValue && attemptedDate > currentEndDate.Value)
                {
                    throw new InvalidDataException("EndDate must be less than the current EndDate.");
                }
            }
        }
    }
}