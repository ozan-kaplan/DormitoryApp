using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.DAL;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            try
            {
                using (DormitoryAppDbContext dbContext = new DormitoryAppDbContext())
                {
                    var userList = dbContext.Users.ToList();
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}