using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCFitnessApp.Models;
using System.IO;

namespace MVCFitnessApp.Controllers
{
    public class ActivityController : Controller
    {
        private Factory db = new Factory();

        // GET: Activity
        public ViewResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "Description_desc" : "Description";

            var activities = from a in db.Activities
                           select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                activities = activities.Where(a => a.Name.Contains(searchString)
                               || a.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    activities = activities.OrderByDescending(a => a.Name);
                    break;
                case "Description":
                    activities = activities.OrderBy(a => a.Description);
                    break;
                case "Description_desc":
                    activities = activities.OrderByDescending(a => a.Description);
                    break;
                default:
                    activities = activities.OrderBy(a => a.Name);
                    break;
            }
            return View(activities.ToList());
        }

        // GET: Activity/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Activity activity = db.Activities.Find(id);

            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activity/Create
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActivityID,Name,Description,PictureFilename")] Activity activity, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // upload file
                if (file != null && file.ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/Pics/Activities/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                }

                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activity);
        }

        // GET: Activity/Edit/5
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityID,Name,Description,PictureFilename")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        // GET: Activity/Delete/5
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activity/Delete/5
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
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
