using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckIndia.UI.Controllers
{
    public class OtherController : Controller
    {
        // GET: Other
        public ActionResult CreateUser()
        {
            var list = new List<SelectListItem>
                    {
                    new SelectListItem{ Text="Administrator", Value = "1" },
                    new SelectListItem{ Text="Customer", Value = "2" },
                    new SelectListItem{ Text="Dealer", Value = "3" },
                    new SelectListItem{ Text="Customer", Value = "4" },
                    new SelectListItem{ Text="Restricted User", Value = "5" },
                    new SelectListItem{ Text="Retailer", Value = "6" },
                    new SelectListItem{ Text="User", Value = "7" },
                    new SelectListItem{ Text="Viewer", Value = "8" },
                    };

            ViewBag.Role = list;


            list = new List<SelectListItem>
                    {
                    new SelectListItem{ Text="Email Address", Value = "1" },
                    new SelectListItem{ Text="Bank Number", Value = "2" },
                    
                    };
            ViewBag.AuthType = list;
                  
             
            return View();
        }
    }
}