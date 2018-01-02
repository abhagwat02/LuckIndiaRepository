using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.DAL.CrudRules.LuckIndia
{
    class AccountCreateRule: CreateRule<Account>
    {
        public AccountCreateRule(Account user, LuckIndiaDBContext context)
            :base(user,context)
        {

        }
        public override void Execute()
        {
            //Context.Update<AccountType>(Model.Type.Id,); 
        }
    }
}
