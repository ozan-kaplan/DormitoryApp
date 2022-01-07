using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.DAL;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {

        public User SessionUser { get; set; }

        public static Logger _logger = LogManager.GetCurrentClassLogger();

        public DormitoryAppDbContext _dbContext = new DormitoryAppDbContext();


        public BaseController()
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        { 

            SessionUser =  CachingHelper.GetUserFromCache(User.Identity.Name); 

            //SessionUser =   Session["User"] as User;

            if (SessionUser == null)
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "Account"));
            }
            else
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

                switch (controllerName)
                {
                    case "DormitoryAnnouncements":
                        if (SessionUser.UserRole == Models.User.Role.Student)
                        {
                            if (actionName == "Create" || actionName == "Edit" || actionName == "DeleteDormitoryAnnouncement")
                            {

                                filterContext.Result = new RedirectResult(Url.Action("Logout", "Account"));
                            }
                        }
                        break;
                    case "Students":
                        if (SessionUser.UserRole == Models.User.Role.Student)
                        {
                            filterContext.Result = new RedirectResult(Url.Action("Logout", "Account"));
                        }
                        break;
                    case "Rooms":
                        if (SessionUser.UserRole == Models.User.Role.Student)
                        {
                            if (actionName == "Create" || actionName == "Edit" || actionName == "DeleteRoom")
                            {

                                filterContext.Result = new RedirectResult(Url.Action("Logout", "Account"));
                            } 
                        }  
                        break;
                    case "Users":
                        if (SessionUser.UserRole == Models.User.Role.Student || SessionUser.UserRole == Models.User.Role.Admin)
                        { 
                             filterContext.Result = new RedirectResult(Url.Action("Logout", "Account")); 
                        } 
                        break;    
                    default:
                        break;
                }   
            }  

            base.OnActionExecuting(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}