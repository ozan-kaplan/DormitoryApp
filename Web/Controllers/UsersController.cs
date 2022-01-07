using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Web.DAL;
using Web.Helpers;
using Web.Models;
using Web.Models.SearchModels;
using Web.Models.ViewModels;

namespace Web.Controllers
{
    public class UsersController : BaseController
    {


        public ActionResult Index()
        {
            return View();
        }
         
        [HttpPost]
        public JsonResult GetUserList(DTParameters param)
        {
            DataTableViewModel<UserViewModel> dataTableViewModel = new DataTableViewModel<UserViewModel>();

            try
            { 
                string direction = string.Empty;
                if (param.Order[0] != null && param.Columns[param.Order[0].Column].Orderable && !string.IsNullOrEmpty(param.SortOrder))
                {
                    direction = param.Order[0].Dir.ToString();
                }
                StudentsSearchModel searchParams = null;
                if (!string.IsNullOrEmpty(param.Search.Value))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    searchParams = js.Deserialize<StudentsSearchModel>(param.Search.Value.ToString());
                }

                var users = from s in _dbContext.Users where !s.IsDeleted && s.UserRole == Models.User.Role.Admin select s;


                if (searchParams != null)
                {
                    if (!string.IsNullOrEmpty(searchParams.SearchText))
                    {
                        users = users.Where(s => s.Email.ToLower().Contains(searchParams.SearchText) ||
                        s.Name.ToLower().Contains(searchParams.SearchText) || s.Lastname.ToLower().Contains(searchParams.SearchText));
                    }

                    if (searchParams.StatusId.HasValue)
                    {
                        var status = (Web.Models.User.UserStatusEnum)searchParams.StatusId.Value;
                        users = users.Where(s => s.UserStatus == status);
                    }
                }

                if (!string.IsNullOrEmpty(direction))
                {
                    if (direction == "ASC")
                        users = users.OrderByField(param.SortOrder, true);
                    else if (direction == "DESC")
                    {
                        users = users.OrderByField(param.SortOrder, false);
                    }
                }
                else
                {
                    users = users.OrderByField("CreatedDate", true);
                }


                var userList = users.Skip(param.Start).Take(param.Length).ToList();


                dataTableViewModel.draw = param.Draw;

                dataTableViewModel.data.AddRange(userList
                    .Select(s => new UserViewModel()
                    {

                        Id = s.Id,
                        Name = s.Name,
                        Lastname = s.Lastname,
                        Email = s.Email,
                        UserGender = s.UserGender.ToString(),
                        UserStatus = s.UserStatus.ToString(),
                        CreatedDate = s.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss")
                    }));

                dataTableViewModel.recordsTotal = users.Count();
                dataTableViewModel.recordsFiltered = dataTableViewModel.recordsTotal;



                return Json(dataTableViewModel);

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Json(dataTableViewModel);
            }
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userDataItem = _dbContext.Users.FirstOrDefault(u => u.Id == id && !u.IsDeleted && u.UserRole == Models.User.Role.Admin);
            UserViewModel userViewModel = null;

            if (userDataItem != null)
            {
                userViewModel = new UserViewModel()
                {  
                    Id = userDataItem.Id,
                    Name = userDataItem.Name,
                    Lastname = userDataItem.Lastname,
                    Email = userDataItem.Email,
                    UserGender = userDataItem.UserGender.ToString(),
                    UserStatus = userDataItem.UserStatus.ToString(),
                    UserRole = userDataItem.UserRole.ToString(),
                    CreatedDate = userDataItem.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss") 
                };
            }
            else
            {
                return HttpNotFound();
            }


            return View(userViewModel);
        }


         

 
         
        public ActionResult Create()
        {
            return View();
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Lastname,Email,Password,UserGender,UserStatus")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.CreatedDate = DateTime.Now;
                    user.CreatedUserId = SessionUser.Id;
                    user.IsDeleted = false;
                    user.UserRole = Models.User.Role.Admin;

                    if (IsEmailUsed(user.Email , user.Id))
                    {
                        ModelState.AddModelError("Email", "This email is using.");
                        return View(user);
                    }



                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return View(user);
        }

        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Lastname,Email,Password,UserGender,UserStatus,CreatedUserId,CreatedDate")] User user)
        {
            if (ModelState.IsValid)
            {
                user.ModifiedDate = DateTime.Now;
                user.ModifiedUserId = SessionUser.Id;
                user.UserRole = Models.User.Role.Admin;


                if (IsEmailUsed(user.Email, user.Id))
                {
                    ModelState.AddModelError("Email", "This email is using.");
                    return View(user);
                }


                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }





        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            JsonResultViewModel<bool> jsonResponse = new JsonResultViewModel<bool>();
            try
            {

                jsonResponse.NotifyType = JsonResultNotifyType.error;
                jsonResponse.Message = "An error occurred while processing your transaction.";


                var item = _dbContext.Users.FirstOrDefault(d => d.Id == id);
                if (item != null)
                {
                    item.IsDeleted = true;
                    item.ModifiedDate = DateTime.Now;
                    item.ModifiedUserId = SessionUser.Id;
                    _dbContext.SaveChanges();

                    jsonResponse.NotifyType = JsonResultNotifyType.info;
                    jsonResponse.Message = "Your transaction has been completed successfully";
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }



        public bool IsEmailUsed(string email, int id)
        {
            return _dbContext.Users.Where(u => !u.IsDeleted && u.Email == email && u.Id != id).Any(); 

        }

    }
}
