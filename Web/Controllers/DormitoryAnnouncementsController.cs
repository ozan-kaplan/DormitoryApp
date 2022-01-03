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

    public class DormitoryAnnouncementsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDormitoryAnnouncementList(DTParameters param)
        {
            DataTableViewModel<DormitoryAnnouncementViewModel> dataTableViewModel = new DataTableViewModel<DormitoryAnnouncementViewModel>();

            try
            {

                string direction = string.Empty;
                if (param.Order[0] != null && param.Columns[param.Order[0].Column].Orderable && !string.IsNullOrEmpty(param.SortOrder))
                {
                    direction = param.Order[0].Dir.ToString();
                }
                BaseSearchModel searchParams = null;
                if (!string.IsNullOrEmpty(param.Search.Value))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    searchParams = js.Deserialize<StudentsSearchModel>(param.Search.Value.ToString());
                }

                var dormitoryAnnouncements = from s in _dbContext.DormitoryAnnouncements where !s.IsDeleted  select s;


                if (searchParams != null)
                {
                    if (!string.IsNullOrEmpty(searchParams.SearchText))
                    {
                        dormitoryAnnouncements = dormitoryAnnouncements.Where(s => s.Announcement.ToLower().Contains(searchParams.SearchText));
                    }
                     
                }

                if (!string.IsNullOrEmpty(direction))
                {
                    if (direction == "ASC")
                        dormitoryAnnouncements = dormitoryAnnouncements.OrderByField(param.SortOrder, true);
                    else if (direction == "DESC")
                    {
                        dormitoryAnnouncements = dormitoryAnnouncements.OrderByField(param.SortOrder, false);
                    }
                }
                else
                {
                    dormitoryAnnouncements = dormitoryAnnouncements.OrderByField("CreatedDate", true);
                }


                var dormitoryAnnouncementList = dormitoryAnnouncements.Skip(param.Start).Take(param.Length).ToList();


                dataTableViewModel.draw = param.Draw;

                dataTableViewModel.data.AddRange(dormitoryAnnouncementList
                    .Select(s => new DormitoryAnnouncementViewModel()
                    {

                        Id = s.Id,
                        Announcement = s.Announcement, 
                        IsPublished = s.IsPublished  ? "YES" : "NO",
                        PublishedDate = s.PublishedDate.HasValue ? s.PublishedDate.Value.ToString("dd.MM.yyyy HH:mm:ss") : ""
                    }));

                dataTableViewModel.recordsTotal = dormitoryAnnouncements.Count();
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
                DormitoryAnnouncement dormitoryAnnouncement = _dbContext.DormitoryAnnouncements.Find(id);
                if (dormitoryAnnouncement == null)
                {
                    return HttpNotFound();
                }
                return View(dormitoryAnnouncement);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Announcement,IsPublished")] DormitoryAnnouncement dormitoryAnnouncement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dormitoryAnnouncement.CreatedDate = DateTime.Now;
                    dormitoryAnnouncement.CreatedUserId = SessionUser.Id;

                    if (dormitoryAnnouncement.IsPublished)
                    {
                        dormitoryAnnouncement.PublishedDate = DateTime.Now;
                    }

                    _dbContext.DormitoryAnnouncements.Add(dormitoryAnnouncement);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(dormitoryAnnouncement);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }



        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DormitoryAnnouncement dormitoryAnnouncement = _dbContext.DormitoryAnnouncements.Find(id);
                if (dormitoryAnnouncement == null)
                {
                    return HttpNotFound();
                }
                return View(dormitoryAnnouncement);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Announcement,IsPublished,CreatedUserId,CreatedDate,PublishedDate")] DormitoryAnnouncement dormitoryAnnouncement)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (dormitoryAnnouncement.IsPublished && !dormitoryAnnouncement.PublishedDate.HasValue)
                    {
                        dormitoryAnnouncement.PublishedDate = DateTime.Now;
                    } 

                    if (!dormitoryAnnouncement.IsPublished)
                    {
                        dormitoryAnnouncement.PublishedDate = null;
                    } 

                    dormitoryAnnouncement.ModifiedDate = DateTime.Now;
                    dormitoryAnnouncement.ModifiedUserId = SessionUser.Id;
                    _dbContext.Entry(dormitoryAnnouncement).State = EntityState.Modified;
                    _dbContext.SaveChanges(); 

                    return RedirectToAction("Index");    
                }
                return View(dormitoryAnnouncement);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        [HttpPost] 
        public JsonResult DeleteDormitoryAnnouncement(int id)
        {
            JsonResultViewModel<bool> jsonResponse = new JsonResultViewModel<bool>();
            try
            {
                
                jsonResponse.NotifyType = JsonResultNotifyType.error;
                jsonResponse.Message = "An error occurred while processing your transaction.";


                var item = _dbContext.DormitoryAnnouncements.FirstOrDefault(d => d.Id == id);
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


         



    }
}
