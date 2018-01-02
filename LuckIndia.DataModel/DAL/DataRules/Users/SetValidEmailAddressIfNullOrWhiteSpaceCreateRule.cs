using System;
using System.Linq;
using System.Text.RegularExpressions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class SetValidEmailAddressIfNullOrWhiteSpaceCreateRule : CreateRule<User>
    {
        public SetValidEmailAddressIfNullOrWhiteSpaceCreateRule(User model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (string.IsNullOrWhiteSpace(this.Model.Email))
                {
                    var firstNameLower = Regex.Replace(this.Model.FirstName.ToLower(), @"\s+", "");
                    var lastNameLower = Regex.Replace(this.Model.LastName.ToLower(), @"\s+", "");

                    var userName = string.Concat(firstNameLower, "-", lastNameLower);

                    var randy = new Random();

                    while (context.Users.Any(u => u.UserName == userName))
                    {
                        userName = string.Concat(firstNameLower, "-", lastNameLower, randy.Next(100, 999999));
                    }

                    this.Model.UserName = userName;
                }
            }
        }
    }
}