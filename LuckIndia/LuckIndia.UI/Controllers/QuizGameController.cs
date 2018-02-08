using LuckIndia.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckIndia.UI.Controllers
{
    public class QuizGameController : Controller
    {
        PlayQuizModel model = new PlayQuizModel(); 
        public QuizGameController()
        {
            model.QuizType.Add(
                "Quiz 1"
                                );

            model.QuizType.Add(
               "Quiz 2"
                               );
            model.QuizType.Add(
               "Quiz 3"
                               );
        }
        // GET: QuizGame
        public ActionResult Index()
        {
            return View(model);
        }
    }
}