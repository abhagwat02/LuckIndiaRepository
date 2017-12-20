using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public String QuizName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ICollection<Question> MyProperty { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}