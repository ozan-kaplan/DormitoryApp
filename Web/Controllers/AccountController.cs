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
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = 1,
                Name = "Ozan",
                Surname = "Kaplan",
                Username = "admin"

            };

            return View(userViewModel);
        }
    }
}