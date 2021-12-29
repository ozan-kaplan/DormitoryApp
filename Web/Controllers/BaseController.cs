using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.DAL;
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
            SessionUser = Session["User"] as User;

            if (SessionUser == null)
            {
                 
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