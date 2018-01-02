using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LuckIndia.DataModel.DAL.CrudRules.LuckIndia
{
    class LuckUserUpdateRule : UpdateRule<LuckUser>
    {
        public LuckUserUpdateRule(LuckUser user, IDictionary<string,object>oldValues,LuckIndiaDBContext context)
            : base(user,oldValues, context)
        {

        }
        public override void Execute()
        {
           // Model.accounts.Select(x => Context.Update((x.Id,OldValues)));
        }
    }
}