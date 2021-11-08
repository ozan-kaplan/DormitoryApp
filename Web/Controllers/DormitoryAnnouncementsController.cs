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
        private DormitoryAppDbContext db = new DormitoryAppDbContext();

        // GET: DormitoryAnnouncements
        public ActionResult Index()
        {
            return View(db.DormitoryAnnouncements.ToList());
        }

        // GET: DormitoryAnnouncements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DormitoryAnnouncement dormitoryAnnouncement = db.DormitoryAnnouncements.Find(id);
            if (dormitoryAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(dormitoryAnnouncement);
        }

        // GET: DormitoryAnnouncements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DormitoryAnnouncements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Announcement,IsPublished")] DormitoryAnnouncement dormitoryAnnouncement)
        {
            if (ModelState.IsValid)
            {

                dormitoryAnnouncement.CreatedDate = DateTime.Now;
                dormitoryAnnouncement.CreatedUserId = SessionUser.Id;

                db.DormitoryAnnouncements.Add(dormitoryAnnouncement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dormitoryAnnouncement);
        }

        // GET: DormitoryAnnouncements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DormitoryAnnouncement dormitoryAnnouncement = db.DormitoryAnnouncements.Find(id);
            if (dormitoryAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(dormitoryAnnouncement);
        }

        // POST: DormitoryAnnouncements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Announcement,IsPublished")] DormitoryAnnouncement dormitoryAnnouncement)
        {
            if (ModelState.IsValid)
            {
                dormitoryAnnouncement.ModifiedDate = DateTime.Now;
                dormitoryAnnouncement.ModifiedUserId = SessionUser.Id;
                db.Entry(dormitoryAnnouncement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dormitoryAnnouncement);
        }

        // GET: DormitoryAnnouncements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DormitoryAnnouncement dormitoryAnnouncement = db.DormitoryAnnouncements.Find(id);
            if (dormitoryAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(dormitoryAnnouncement);
        }

        // POST: DormitoryAnnouncements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DormitoryAnnouncement dormitoryAnnouncement = db.DormitoryAnnouncements.Find(id);
            db.DormitoryAnnouncements.Remove(dormitoryAnnouncement);
            db.SaveChanges();
            return RedirectToAction("Index");
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
