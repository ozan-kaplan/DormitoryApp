using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {

        public User SessionUser { get; set; }
        public BaseController()
        {
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SessionUser = Session["User"] as User;
            base.OnActionExecuting(filterContext);
        }
    }
}