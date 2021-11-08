using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            User userViewModel = new User()
            {
                Id = 1,
                Name = "Ozan",
                Lastname = "Kaplan",
                Email = "admin" 
            };

            return View(userViewModel);
        }
    }
}