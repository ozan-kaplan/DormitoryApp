using NLog;
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
using Web.Models.ViewModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private DormitoryAppDbContext _dbContext = new DormitoryAppDbContext();
        public static Logger _logger = LogManager.GetCurrentClassLogger();


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
        public ActionResult ForgotPassword()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([Bind(Include = "Email,Password")] UserLoginViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userItem = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                    if (userItem != null)
                    {
                        if (userItem.UserStatus != Models.User.UserStatusEnum.Active)
                        {
                            ViewBag.LoginError = "Your account has not been activated.";
                        }
                        else
                        {
                            CachingHelper.AddUserToCache(userItem.Email, userItem);
                            FormsAuthentication.SetAuthCookie(userItem.Email, false);
                            return RedirectToAction("Index", "Home");

                        }
                    }
                    else
                    {
                        ViewBag.LoginError = "Username or password is wrong.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ViewBag.LoginError = "An error occurred while processing your transaction.";
            }

            return View(user);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register([Bind(Include = "Name,Lastname,Email,Password,UserGender")] UserRegisterViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (IsEmailUsed(user.Email))
                    {
                        ModelState.AddModelError("Email", "This email is using.");
                        return View(user);
                    }

                    var userDataItem = new User
                    {
                        Name = user.Name,
                        Lastname = user.Lastname,
                        Email = user.Email,
                        Password = user.Password,
                        UserGender = user.UserGender,
                        UserStatus = Models.User.UserStatusEnum.Pending,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = -2,
                        IsDeleted = false
                    };

                    _dbContext.Users.Add(userDataItem);
                    _dbContext.SaveChanges();


                    EmailHelper.Send(EmailHelper.EmailType.StudentStatusChange, user.Email, "Your student registration has been created, please wait for it to be approved by your administrator. You will be notified of the registration result by e-mail.");



                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ModelState.AddModelError("Name", "An error occurred while processing your transaction.");
                return View(user);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword([Bind(Include = "Email")] UserForgotPasswordViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userItem = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email && !u.IsDeleted);
                    if (userItem != null)
                    {

                        EmailHelper.Send(EmailHelper.EmailType.PasswordReminder, userItem.Email, "Your password is " + userItem.Password);
                        ModelState.AddModelError("Email", "Password reminder email has been sent.");
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "This email is not registered in the system.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ModelState.AddModelError("Email", "An error occurred while processing your transaction.");
            }

            return View(user);
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }



        public bool IsEmailUsed(string email)
        {
            return _dbContext.Users.Where(u => !u.IsDeleted && u.Email == email).Any();

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