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
    public class RoomApplicationsController : BaseController
    {
        private DormitoryAppDbContext db = new DormitoryAppDbContext();

        // GET: RoomApplications
        public ActionResult Index()
        {
            return View(db.RoomApplications.ToList());
        }

        // GET: RoomApplications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomApplication roomApplication = db.RoomApplications.Find(id);
            if (roomApplication == null)
            {
                return HttpNotFound();
            }
            return View(roomApplication);
        }

        // GET: RoomApplications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,RoomId,ApplyDate,AccommodationStartDate,AccodomodationEndDate,PaymentDate,CreatedUserId,CreatedDate,ModifiedUserId,ModifiedDate")] RoomApplication roomApplication)
        {
            if (ModelState.IsValid)
            {
                db.RoomApplications.Add(roomApplication);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roomApplication);
        }

        // GET: RoomApplications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomApplication roomApplication = db.RoomApplications.Find(id);
            if (roomApplication == null)
            {
                return HttpNotFound();
            }
            return View(roomApplication);
        }

        // POST: RoomApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,RoomId,ApplyDate,AccommodationStartDate,AccodomodationEndDate,PaymentDate,CreatedUserId,CreatedDate,ModifiedUserId,ModifiedDate")] RoomApplication roomApplication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomApplication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomApplication);
        }

        // GET: RoomApplications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomApplication roomApplication = db.RoomApplications.Find(id);
            if (roomApplication == null)
            {
                return HttpNotFound();
            }
            return View(roomApplication);
        }

        // POST: RoomApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomApplication roomApplication = db.RoomApplications.Find(id);
            db.RoomApplications.Remove(roomApplication);
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
