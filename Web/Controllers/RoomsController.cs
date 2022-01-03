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


    public class RoomsController : BaseController
    {



        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public JsonResult GetRoomList(DTParameters param)
        {
            DataTableViewModel<RoomViewModel> dataTableViewModel = new DataTableViewModel<RoomViewModel>();

            try
            {

                string direction = string.Empty;
                if (param.Order[0] != null && param.Columns[param.Order[0].Column].Orderable && !string.IsNullOrEmpty(param.SortOrder))
                {
                    direction = param.Order[0].Dir.ToString();
                }
                RoomSearchModel searchParams = null;
                if (!string.IsNullOrEmpty(param.Search.Value))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    searchParams = js.Deserialize<RoomSearchModel>(param.Search.Value.ToString());
                }

                var rooms = from r in _dbContext.Rooms where !r.IsDeleted select r;


                if (searchParams != null)
                {
                    if (!string.IsNullOrEmpty(searchParams.SearchText))
                    {
                        rooms = rooms.Where(s => s.RoomName.ToLower().Contains(searchParams.SearchText));
                    }

                    if (searchParams.StatusId.HasValue)
                    {

                        if ((RoomStatus)searchParams.StatusId.Value == RoomStatus.Available)
                        {
                            rooms = rooms.Where(s => s.RoomCapacity != s.CurrentCapacity);
                        }
                        else if ((RoomStatus)searchParams.StatusId.Value == RoomStatus.Unavailable)
                        {
                            rooms = rooms.Where(s => s.RoomCapacity == s.CurrentCapacity);
                        }

                    }
                }

                if (!string.IsNullOrEmpty(direction))
                {
                    if (direction == "ASC")
                        rooms = rooms.OrderByField(param.SortOrder, true);
                    else if (direction == "DESC")
                    {
                        rooms = rooms.OrderByField(param.SortOrder, false);
                    }
                }
                else
                {
                    rooms = rooms.OrderByField("Id", true);
                }


                var roomList = rooms.Skip(param.Start).Take(param.Length).ToList();


                dataTableViewModel.draw = param.Draw;

                dataTableViewModel.data.AddRange(roomList
                    .Select(s => new RoomViewModel()
                    {

                        Id = s.Id,
                        RoomName = s.RoomName,
                        RoomCapacity = s.RoomCapacity,
                        CurrentCapacity = s.CurrentCapacity,
                        RoomFee = s.RoomFee,
                        Status = s.RoomCapacity == s.CurrentCapacity ? RoomStatus.Unavailable.ToString() : RoomStatus.Available.ToString()

                    }));

                dataTableViewModel.recordsTotal = rooms.Count();
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
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Room room = _dbContext.Rooms.Find(id);
                if (room == null)
                {
                    return HttpNotFound();
                }
                return View(room);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return RedirectToAction("Index" , "Rooms");
        }
         
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RoomName,RoomFee,RoomCapacity")] Room room)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    room.CreatedDate = DateTime.Now;
                    room.CreatedUserId = SessionUser.Id;
                    _dbContext.Rooms.Add(room);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return View(room);
        }

       
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Room room = _dbContext.Rooms.Find(id);
                if (room == null)
                {
                    return HttpNotFound();
                }
                return View(room);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return RedirectToAction("Create", "Rooms");
            
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RoomName,RoomFee,RoomCapacity")] RoomViewModel room)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dataItem = _dbContext.Rooms.Find(room.Id);

                    if (dataItem != null)
                    {
                        dataItem.ModifiedDate = DateTime.Now;
                        dataItem.ModifiedUserId = SessionUser.Id;
                        dataItem.RoomName = room.RoomName;
                        dataItem.RoomFee = room.RoomFee;
                        dataItem.RoomCapacity = room.RoomCapacity;

                        _dbContext.Entry(dataItem).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
               
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return View(room);
        }


        [HttpPost]
        public JsonResult DeleteRoom(int id)
        {
            JsonResultViewModel<bool> jsonResponse = new JsonResultViewModel<bool>();
            try
            {

                jsonResponse.NotifyType = JsonResultNotifyType.error;
                jsonResponse.Message = "An error occurred while processing your transaction.";


                var item = _dbContext.Rooms.FirstOrDefault(d => d.Id == id);
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


        [HttpPost]
        public JsonResult ApplyRoom(RoomApplication roomApplication)
        {
            JsonResultViewModel<bool> jsonResponse = new JsonResultViewModel<bool>();
            try
            {

                jsonResponse.NotifyType = JsonResultNotifyType.error;
                jsonResponse.Message = "An error occurred while processing your transaction.";



                var hasUserRoomApplication  = _dbContext.RoomApplications.FirstOrDefault(d => d.UserId == SessionUser.Id);
                if (hasUserRoomApplication != null)
                {
                    jsonResponse.Message = "You already have a room application. Please wait your application result."; 
                    return Json(jsonResponse, JsonRequestBehavior.AllowGet);
                }

                var item = _dbContext.Rooms.FirstOrDefault(d => d.Id == roomApplication.Id);
                if (item != null)
                {

                    RoomApplication roomApplicationDataItem = new RoomApplication()
                    {
                        CreatedDate = DateTime.Now,
                        CreatedUserId = SessionUser.Id,
                        UserId = SessionUser.Id,
                        RoomId = roomApplication.Id,
                        AccommodationStartDate = roomApplication.AccommodationStartDate,
                        AccodomodationEndDate = roomApplication.AccodomodationEndDate,
                        RoomApplicationStatus = RoomApplication.Status.Pending,
                        ApplyDate = DateTime.Now,
                    };

                    _dbContext.RoomApplications.Add(roomApplicationDataItem);
                    _dbContext.SaveChanges();

                    jsonResponse.NotifyType = JsonResultNotifyType.info;
                    jsonResponse.Message = "Your transaction has been completed successfully. You can track your application in My room application page.";
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
