using System;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class DateOfBirthCreateRule : CreateRule<User>
    {
        public DateOfBirthCreateRule(User model) : base(model) { }

        public override void Validate()
        {
            if(this.Model.DateOfBirth.HasValue && this.Model.DateOfBirth.Value.CompareTo(DateTime.UtcNow) > 0 )
            {
                throw new InvalidDataException("Date of birth can not be in the future");
            }
        }
    }
}