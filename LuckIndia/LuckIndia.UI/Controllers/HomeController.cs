using LuckIndia.Models.DTO;
using LuckIndia.Services.RegistrationServices;
using LuckIndia.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckIndia.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Welcome to Luck India";
                
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {

            if (Session["UserID"] == null)
            {
                if (ModelState.IsValid)
                {
                    Registration reg = new Registration();

                    string strError = "";
                    UserDto user = reg.SignIn(model.UserName, model.Password, out strError);
                    if (user != null)
                    {
                        Session["UserID"] = model.UserName;
                        return RedirectToAction("Index", "Quiz");

                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect username or password");
                    }
                    //ViewBag.Success = bSuccess ? "Successful login" : strError;
                    //if (bSuccess)
                    //    return RedirectToAction("About");
                    //else

                }
            }

            ModelState.AddModelError("", "User already logged in");

            return RedirectToAction("Login");

        }


        public ActionResult SignIn(LoginModel model)
        {
           
            return View(model);
        }

      
    }
}
