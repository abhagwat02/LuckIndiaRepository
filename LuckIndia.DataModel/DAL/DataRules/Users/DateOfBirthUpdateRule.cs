using System;
using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class DateOfBirthUpdateRule : UpdateRule<User>
    {
        public DateOfBirthUpdateRule(User model, IDictionary<string, object> delta) : base(model, delta) { }

        public override void Validate()
        {
            var dateOfBirthKey = DAL.PropertyName<User>(x => x.DateOfBirth);
            if(!this.Delta.ContainsKey(dateOfBirthKey))
            {
                return;
            }

            var updatedBirthday = this.Delta[dateOfBirthKey] as DateTime?;

            if(updatedBirthday.HasValue && updatedBirthday.Value.CompareTo(DateTime.UtcNow) > 0)
            {
                throw new InvalidDataException("Date of birth can not be in the future");
            }            
        }
    }
}