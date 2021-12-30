using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web.DAL;
using Web.Models;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private DormitoryAppDbContext db = new DormitoryAppDbContext();
         
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
        public ActionResult Login ([Bind(Include = "Email,Password")] UserLoginModel user)
        {
            if (ModelState.IsValid)
            { 
                var userItem =  db.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (userItem != null)
                { 
                    Session["User"] = userItem; 
                    return RedirectToAction("Index","Home");
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
            Session["User"] = null;
            return RedirectToAction("Login");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}