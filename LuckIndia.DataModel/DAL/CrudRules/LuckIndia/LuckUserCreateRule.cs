using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.DAL.CrudRules.LuckIndia
{
    class LuckUserCreateRule : CreateRule<LuckUser>
    {
        public LuckUserCreateRule(LuckUser user, LuckIndiaDBContext context)
            :base(user,context)
        {
    
        }
        public override void Execute()
        {
            Model.accounts.Select(x => Context.Create((x)));
        }
    }
}
