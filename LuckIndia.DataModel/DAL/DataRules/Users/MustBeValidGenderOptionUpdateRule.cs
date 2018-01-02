using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class MustBeValidGenderOptionUpdateRule: UpdateRule<User>
    {
        public MustBeValidGenderOptionUpdateRule(User model, IDictionary<string, object> delta) 
            : base(model, delta)
        {
        }

        private readonly HashSet<string> _validGenders = new HashSet<string> { "M", "F", "U", "T" };

        public override void Validate()
        {
            var genderKey = DAL.PropertyName<User>(x => x.Gender);
            if (!this.Delta.ContainsKey(genderKey))
            {
                return;
            }

            var gender = this.Delta[genderKey].ToString().ToUpper();

            if (!_validGenders.Contains(gender))
            {
                throw new InvalidDataException(string.Format("Gender must be {0}.", string.Join(", ", _validGenders)));
            }

            this.Delta[genderKey] = gender;

        }
    }
}