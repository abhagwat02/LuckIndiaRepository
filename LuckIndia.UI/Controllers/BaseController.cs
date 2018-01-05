using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckIndia.UI.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserName"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult { Data = "LogOut", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                    filterContext.Result = new RedirectResult("~/Home/Login");
            }

        }

    }
}
