using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckIndia.UI.Models
{
    public class PlayQuizModel
    {
        public PlayQuizModel()
        {
            QuizType = new List<string>();
        }
        public List<String> QuizType { get; set; }

        [Display(Name = "Story:")]
        public string Story { get; set; }

        [Display(Name = "Title Zero:")]
        public string Option0 { get; set; }

        [Display(Name = "Title One:")]
        public string Option1 { get; set; }

        [Display(Name = "Title Two:")]
        public string Option2 { get; set; }

        [Display(Name = "Title Three:")]
        public string Option3 { get; set; }

        [Display(Name = "Title Four:")]
        public string Option4 { get; set; }

        [Display(Name = "Title Five:")]
        public string Option5 { get; set; }

        [Display(Name = "Title Six:")]
        public string Option6 { get; set; }

        [Display(Name = "Title Seven:")]
        public string Option7 { get; set; }

        [Display(Name = "Title Eight:")]
        public string Option8 { get; set; }

        [Display(Name = "Title Nine:")]
        public string Option9 { get; set; }

        [Display(Name = "Title Cross:")]
        public string Option10 { get; set; }
    }
}
