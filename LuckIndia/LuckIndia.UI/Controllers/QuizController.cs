using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckIndia.UI.Controllers
{
    public class QuizController : Controller
    {
        //
        // GET: /Quiz/

        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


    }
}
