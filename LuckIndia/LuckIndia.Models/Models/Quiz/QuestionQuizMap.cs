using LuckIndia.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.Models
{
    [Include]
    public class QuestionQuizMap : Model
    {
        public override int Id { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        [Include]
        public Question question { get; set; }
        [Include]
        public Quiz quiz { get; set; }


    }
}
