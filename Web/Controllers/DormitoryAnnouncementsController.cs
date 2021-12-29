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

        public ActionResult Index()
        {
            try
            {
                return View(_dbContext.DormitoryAnnouncements.ToList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
        public ActionResult Create([Bind(Include = "Id,Announcement,IsPublished")] DormitoryAnnouncement dormitoryAnnouncement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dormitoryAnnouncement.CreatedDate = DateTime.Now;
                    dormitoryAnnouncement.CreatedUserId = SessionUser.Id;


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


        public ActionResult Delete(int? id)
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

       
        public ActionResult Delete(int id)
        {
            try
            {
                DormitoryAnnouncement dormitoryAnnouncement = _dbContext.DormitoryAnnouncements.Find(id);
                
                _dbContext.DormitoryAnnouncements.Remove(dormitoryAnnouncement);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


    }
}
