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
    public class RoomApplicationsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetRoomApplicationList(DTParameters param)
        {
            DataTableViewModel<RoomApplicationViewModel> dataTableViewModel = new DataTableViewModel<RoomApplicationViewModel>();

            try
            {



                string direction = string.Empty;
                if (param.Order[0] != null && param.Columns[param.Order[0].Column].Orderable && !string.IsNullOrEmpty(param.SortOrder))
                {
                    direction = param.Order[0].Dir.ToString();
                }
                RoomApplicationSearchModel searchParams = null;
                if (!string.IsNullOrEmpty(param.Search.Value))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    searchParams = js.Deserialize<RoomApplicationSearchModel>(param.Search.Value.ToString());
                }

                var roomApplication = from r in _dbContext.RoomApplications where !r.IsDeleted select r;


                if (SessionUser.UserRole == Models.User.Role.Student)
                {
                    roomApplication = roomApplication.Where(r => r.UserId == SessionUser.Id);
                }

                if (searchParams != null)
                {

                    if (searchParams.StatusId.HasValue)
                    {
                        var status = (RoomApplication.RoomApplicationStatusEnum)searchParams.StatusId.Value;
                        roomApplication = roomApplication.Where(s => s.RoomApplicationStatus == status);
                    }

                    if (searchParams.StartDate.HasValue)
                    {
                        roomApplication = roomApplication.Where(s => s.ApplyDate >= searchParams.StartDate.Value);
                    }

                    if (searchParams.EndDate.HasValue)
                    {
                        roomApplication = roomApplication.Where(s => s.ApplyDate >= searchParams.EndDate.Value);
                    }
                }
                else
                {
                    DateTime startDate = DateTime.Now.AddDays(-10);
                    DateTime endDate = DateTime.Now;

                    roomApplication = roomApplication.Where(s => s.ApplyDate >= startDate && s.ApplyDate <= endDate);
                }

                var joinedQuery = (from roomApp in roomApplication
                                   join user in _dbContext.Users on roomApp.UserId equals user.Id
                                   join room in _dbContext.Rooms on roomApp.RoomId equals room.Id
                                   where !roomApp.IsDeleted && !user.IsDeleted && !room.IsDeleted && user.UserRole == Models.User.Role.Student
                                   select new RoomApplicationViewModel
                                   {
                                       Id = roomApp.Id,
                                       UserFullName = user.Name + " " + user.Lastname,
                                       RoomName = room.RoomName,
                                       ApplyDate = roomApp.ApplyDate,
                                       AccommodationStartDate = roomApp.AccommodationStartDate,
                                       AccodomodationEndDate = roomApp.AccodomodationEndDate,
                                       PaymentDate = roomApp.PaymentDate,
                                       RoomApplicationStatus = roomApp.RoomApplicationStatus.ToString()
                                   });


                if (!string.IsNullOrEmpty(direction))
                {



                    if (direction == "ASC")
                        joinedQuery = joinedQuery.OrderByField(param.SortOrder, true);
                    else if (direction == "DESC")
                    {
                        joinedQuery = joinedQuery.OrderByField(param.SortOrder, false);
                    }
                }
                else
                {
                    joinedQuery = joinedQuery.OrderByField("Id", false);
                }


                var roomApplicationList = joinedQuery.Skip(param.Start).Take(param.Length).ToList();


                dataTableViewModel.draw = param.Draw;

                dataTableViewModel.data.AddRange(roomApplicationList);

                dataTableViewModel.recordsTotal = roomApplicationList.Count();
                dataTableViewModel.recordsFiltered = dataTableViewModel.recordsTotal;



                return Json(dataTableViewModel);

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Json(dataTableViewModel);
            }
        }



        [HttpPost]
        public JsonResult ChangeApplicationStatus(int id, int status)
        {
            JsonResultViewModel<bool> jsonResponse = new JsonResultViewModel<bool>();
            try
            {

                jsonResponse.NotifyType = JsonResultNotifyType.error;
                jsonResponse.Message = "An error occurred while processing your transaction.";

                var statusEnum = (RoomApplication.RoomApplicationStatusEnum)status;
                if (SessionUser.UserRole == Models.User.Role.Student && (statusEnum != RoomApplication.RoomApplicationStatusEnum.Cancelled || statusEnum != RoomApplication.RoomApplicationStatusEnum.PaymentCompleted))
                {
                    jsonResponse.Message = "You dont have this permission!";
                    return Json(jsonResponse, JsonRequestBehavior.AllowGet);
                }

                var item = _dbContext.RoomApplications.FirstOrDefault(d => d.Id == id);
                if (item != null)
                {
                    item.ModifiedDate = DateTime.Now;
                    item.ModifiedUserId = SessionUser.Id;
                    item.RoomApplicationStatus = statusEnum;


                    if (item.RoomApplicationStatus == RoomApplication.RoomApplicationStatusEnum.Approved)
                    {

                        var room = _dbContext.Rooms.FirstOrDefault(u => !u.IsDeleted && u.Id == item.RoomId);

                        if (room.CurrentCapacity < room.RoomCapacity)
                        {
                            var student = _dbContext.Users.FirstOrDefault(u => !u.IsDeleted && u.Id == item.UserId);
                            if (student != null)
                            {
                                if (student.RoomId.HasValue)
                                {
                                    var oldRoom = _dbContext.Rooms.FirstOrDefault(u => !u.IsDeleted && u.Id == item.RoomId);
                                    if (oldRoom != null)
                                    {
                                        oldRoom.CurrentCapacity--;
                                        if (oldRoom.CurrentCapacity < 0)
                                        {
                                            oldRoom.CurrentCapacity = 0;
                                        }
                                    }
                                }
                                room.CurrentCapacity++;
                                student.RoomId = item.RoomId; //place student to the room
                            }
                        }
                        else
                        {
                            jsonResponse.NotifyType = JsonResultNotifyType.error;
                            jsonResponse.Message = "Room capacity is not enough.";
                            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
                        }



                    }


                    _dbContext.SaveChanges();
                    jsonResponse.NotifyType = JsonResultNotifyType.info;
                    jsonResponse.Message = "Your transaction has been completed successfully";
                    jsonResponse.ResponseData = true;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult MakePayment(int id)
        {
            JsonResultViewModel<bool> jsonResponse = new JsonResultViewModel<bool>();
            try
            {

                jsonResponse.NotifyType = JsonResultNotifyType.error;
                jsonResponse.Message = "An error occurred while processing your transaction.";


                var item = _dbContext.RoomApplications.FirstOrDefault(d => d.Id == id);
                if (item != null)
                {
                    item.ModifiedDate = DateTime.Now;
                    item.ModifiedUserId = SessionUser.Id;
                    item.PaymentDate = DateTime.Now;
                    item.RoomApplicationStatus = RoomApplication.RoomApplicationStatusEnum.PaymentCompleted;

                    _dbContext.SaveChanges();
                    jsonResponse.NotifyType = JsonResultNotifyType.info;
                    jsonResponse.Message = "Your transaction has been completed successfully";
                    jsonResponse.ResponseData = true;
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
