using LuckIndia.Services.QuizServices;
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
            QuizService service = new QuizService();
            var quiztypes = service.GetQuizTypes();
            if (quiztypes != null)
            {
                var result = quiztypes.Result;
                result.ToList().ForEach
                (
                m => model.QuizType.Add(m)
                );
            }

        }
        // GET: QuizGame
        public ActionResult Index()
        {
            return View(model);
        }
    }
}