using PagedList;
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
    public class StudentsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetStudentsList(DTParameters param)
        {
            DataTableViewModel<StudentsViewModel> dataTableViewModel = new DataTableViewModel<StudentsViewModel>();

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

                var students = from s in _dbContext.Users where !s.IsDeleted && s.UserRole == Models.User.Role.Student select s;


                if (searchParams != null)
                {
                    if (!string.IsNullOrEmpty(searchParams.SearchText))
                    {
                        students = students.Where(s => s.Email.ToLower().Contains(searchParams.SearchText) || s.Name.ToLower().Contains(searchParams.SearchText) || s.Lastname.ToLower().Contains(searchParams.SearchText));
                    }

                    if (searchParams.StatusId.HasValue)
                    {
                        var status = (Web.Models.User.UserStatusEnum)searchParams.StatusId.Value;
                        students = students.Where(s => s.UserStatus == status);
                    }
                }

                if (!string.IsNullOrEmpty(direction))
                {
                    if (direction == "ASC")
                        students = students.OrderByField(param.SortOrder, true);
                    else if (direction == "DESC")
                    {
                        students = students.OrderByField(param.SortOrder, false);
                    }
                }
                else
                {
                    students = students.OrderByField("CreatedDate", true);
                }


                var studentList = students.Skip(param.Start).Take(param.Length).ToList();


                dataTableViewModel.draw = param.Draw;

                dataTableViewModel.data.AddRange(studentList
                    .Select(s => new StudentsViewModel()
                    {

                        Id = s.Id,
                        Name = s.Name,
                        Lastname = s.Lastname,
                        Email = s.Email,
                        UserGender = s.UserGender.ToString(),
                        UserStatus = s.UserStatus.ToString(),
                        CreatedDate = s.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss")
                    }));

                dataTableViewModel.recordsTotal = students.Count();
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
            var studentDataItem = _dbContext.Users.FirstOrDefault(u => u.Id == id && !u.IsDeleted && u.UserRole == Models.User.Role.Student);
            StudentsViewModel studentViewModel = null;

            if (studentDataItem != null)
            {
                studentViewModel = new StudentsViewModel()
                {


                    Id = studentDataItem.Id,
                    Name = studentDataItem.Name,
                    Lastname = studentDataItem.Lastname,
                    Email = studentDataItem.Email,
                    UserGender = studentDataItem.UserGender.ToString(),
                    UserStatus = studentDataItem.UserStatus.ToString(),
                    UserRole = studentDataItem.UserRole.ToString(),
                    CreatedDate = studentDataItem.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss")

                };
            }
            else
            {
                return HttpNotFound();
            }


            return View(studentViewModel);
        }




        [HttpPost]

        public JsonResult ChangeStudentStatus(int id, int userStatus)
        {
            JsonResultViewModel<bool> jsonResponse = new JsonResultViewModel<bool>();
            try
            {

                jsonResponse.NotifyType = JsonResultNotifyType.error;
                jsonResponse.Message = "An error occurred while processing your transaction.";


                var item = _dbContext.Users.FirstOrDefault(d => d.Id == id && d.UserRole == Models.User.Role.Student);
                if (item != null)
                {
                    item.UserStatus = (User.UserStatusEnum)userStatus;
                    item.ModifiedDate = DateTime.Now;
                    item.ModifiedUserId = SessionUser.Id;
                    _dbContext.SaveChanges();

                    jsonResponse.NotifyType = JsonResultNotifyType.info;
                    jsonResponse.Message = "Your transaction has been completed successfully";
                    jsonResponse.ResponseData = true;




                    switch (item.UserStatus)
                    {
                        case Models.User.UserStatusEnum.Active:
                            EmailHelper.Send(EmailHelper.EmailType.StudentStatusChange, item.Email, "Your account has been activated. You can login to the system.");
                            break;
                        case Models.User.UserStatusEnum.Passive:
                            EmailHelper.Send(EmailHelper.EmailType.StudentStatusChange, item.Email, "Your account has been deactivated. You will not be able to login to the system until your account is reactivated.");
                            break;
                        default:
                            break;
                    }



                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }



    }
}
