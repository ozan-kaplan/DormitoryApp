using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using Web.DAL;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private DormitoryAppDbContext _dbContext = new DormitoryAppDbContext();


        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([Bind(Include = "Email,Password")] UserLoginModel user)
        {
            if (ModelState.IsValid)
            {
                var userItem = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (userItem != null)
                { 
                    CachingHelper.AddUserToCache(userItem.Email, userItem); 
                    FormsAuthentication.SetAuthCookie(userItem.Email, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.LoginError = "Kullanıcı adı veya şifreniz hatalıdır.";
                }
            }

            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
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