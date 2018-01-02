using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class MustBeValidGenderOptionCreateRule: CreateRule<User>
    {
        public MustBeValidGenderOptionCreateRule(User model) 
            : base(model)
        {
        }

        private readonly HashSet<string> _validGenders = new HashSet<string>{"M","F","U","T"};

        public override void Validate()
        {
            if (null == Model.Gender)
            {
                return;
            }

            var gender = this.Model.Gender.ToUpper();

            if (!_validGenders.Contains(gender))
            {
                throw new InvalidDataException(string.Format("Gender must be {0}.", string.Join(", ", _validGenders)));
            }

            this.Model.Gender = gender;
        }
    }
}