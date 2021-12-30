using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.DAL;
using Web.Models;

namespace Web.Controllers
{

    public class DormitoryAnnouncementsController : BaseController
    {



        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
           

            ViewBag.CurrentSort = sortOrder;
            if (sortOrder != null)
            {
                ViewBag.PublishedDateSortParm = sortOrder == "published_date_asc" ? "published_date_desc" : "published_date_asc";
            }
            else
            {
                ViewBag.PublishedDateSortParm = "published_date_desc";
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var announcements = from a in _dbContext.DormitoryAnnouncements
                           select a;
            if (!String.IsNullOrEmpty(searchString))
            {
                announcements = announcements.Where(s => s.Announcement.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "published_date_desc":
                    announcements = announcements.OrderByDescending(s => s.PublishedDate);
                    break;
                case "published_date_asc":
                    announcements = announcements.OrderBy(s => s.PublishedDate); 
                    break; 
                default:  
                    announcements = announcements.OrderByDescending(s => s.Id);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(announcements.ToPagedList(pageNumber, pageSize));
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


       

       

        public ActionResult Delete(int id)
        {
            try
            {
                DormitoryAnnouncement dormitoryAnnouncement = _dbContext.DormitoryAnnouncements.Find(id);
                if (dormitoryAnnouncement != null)
                {
                    _dbContext.DormitoryAnnouncements.Remove(dormitoryAnnouncement);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


    }
}
