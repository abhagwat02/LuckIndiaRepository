using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel.DAL.CrudRules.LuckIndia
{
    class QuestionCreateRule : CreateRule<Question>
    {
        public QuestionCreateRule(Question question, LuckIndiaDBContext context)
            :base(question,context)
        {

        }
        public override void Execute()
        {
            Model.Options.Select(x => Context.Create((x)));
        }
    }
}
