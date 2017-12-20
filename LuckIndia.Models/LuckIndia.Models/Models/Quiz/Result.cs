using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.Models
{
    public class Result
    {
        public int Id { get; set; }
        public DateTime AnnouncementDate { get; set; }

        public Option CorrectOption { get; set; }
        public Quiz SelectedQuiz { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}