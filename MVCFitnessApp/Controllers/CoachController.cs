using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCFitnessApp.Models;
using MVCFitnessApp.ViewModels;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace MVCFitnessApp.Controllers
{
    public class CoachController : Controller
    {
        private Factory db = new Factory();

        // GET: Coach
        public ActionResult Index(int? id, int? activityID)
        {
            var viewModel = new CoachIndexData();
            viewModel.Coachs = db.Coachs
                .Include(c => c.Activities.Select(m => m.Members))
                .OrderBy(c => c.LastName);

            if (id != null)
            {
                ViewBag.CoachID = id.Value;
                viewModel.Activities = viewModel.Coachs.Where(
                    c => c.CoachID == id.Value).Single().Activities;
            }

            if (activityID != null)
            {
                ViewBag.ActivityID = activityID.Value;
                //// Lazy loading 
                //viewModel.Activities = viewModel.Members.Where(
                //    m => m.MemberID == id.Value).Single().Activities;

                // Explicit loading 
                var selectedActivity = viewModel.Activities.Where(x => x.ActivityID == activityID).Single();
                db.Entry(selectedActivity).Collection(x => x.Members).Load();

                foreach (Member member in selectedActivity.Members)
                {
                    db.Entry(member).Collection(x => x.Activities).Load();
                }

                viewModel.Members = selectedActivity.Members;
            }

            return View(viewModel);
        }

        // GET: Coach/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Coach coach = db.Coachs.Include(c => c.FilePaths).SingleOrDefault(c => c.CoachID == id);

            if (coach == null)
            {
                return HttpNotFound();
            }
            return View(coach);
        }

        // GET: Coach/Create
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Create()
        {
            var coach = new Coach();
            coach.Activities = new List<Activity>();
            PopulateAssignedActivityData(coach);
            return View();
        }

        // POST: Coach/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CoachID,LastName,FirstName,HireDate")] Coach coach, string[] selectedActivities, HttpPostedFileBase upload)
        {
            if (selectedActivities != null)
            {
                coach.Activities = new List<Activity>();
                foreach (var activity in selectedActivities)
                {
                    var activityToAdd = db.Activities.Find(int.Parse(activity));
                    coach.Activities.Add(activityToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var photo = new FilePath
                    {
                        FileName = Path.GetFileName(upload.FileName),
                        FileType = "Photo"
                    };

                    string path = Path.Combine(Server.MapPath("~/Pics/Coach/"), Path.GetFileName(upload.FileName));

                    upload.SaveAs(path);
                    coach.FilePaths = new List<FilePath>();
                    coach.FilePaths.Add(photo);
                }

                db.Coachs.Add(coach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssignedActivityData(coach);
            return View(coach);
        }

        // GET: Coach/Edit/5
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Coach coach = db.Coachs
                .Include(a => a.Activities)
                .Where(a => a.CoachID == id)
                .Single();

            PopulateAssignedActivityData(coach);

            if (coach == null)
            {
                return HttpNotFound();
            }
            return View(coach);
        }

        private void PopulateAssignedActivityData(Coach coach)
        {
            var allActivities = db.Activities;
            var CoachActivities = new HashSet<int>(coach.Activities.Select(c => c.ActivityID));
            var viewModel = new List<AssignedActivityData>();

            foreach (var activity in allActivities)
            {
                //viewmodel to get ActivityID and Name from Activity
                viewModel.Add(new AssignedActivityData
                {
                    ActivityID = activity.ActivityID,
                    Name = activity.Name,
                    Assigned = CoachActivities.Contains(activity.ActivityID)
                });
            }
            ViewBag.Activities = viewModel;
        }

        // POST: Coach/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedActivities, HttpPostedFileBase upload)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var coachToUpdate = db.Coachs
               .Include(c => c.Activities)
               .Where(c => c.CoachID == id)
               .Include(c => c.FilePaths)            
               .Single();

            if (TryUpdateModel(coachToUpdate, "",
               new string[] { "LastName", "FirstName", "HireDate" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (coachToUpdate.FilePaths.Any(f => f.FileType == "Photo"))
                        {
                            db.FilePaths.Remove(coachToUpdate.FilePaths.First(f => f.FileType == "Photo"));
                        }

                        var photo = new FilePath
                        {
                            FileName = Path.GetFileName(upload.FileName),
                            //FileName = Path.GetFileName(upload.FileName),
                            FileType = "Photo",
                        };

                        string path = Path.Combine(Server.MapPath("~/Pics/Coach/"), Path.GetFileName(upload.FileName));

                        upload.SaveAs(path);
                        coachToUpdate.FilePaths = new List<FilePath> { photo };
                    }

                    UpdateCoachActivities(selectedActivities, coachToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedActivityData(coachToUpdate);
            return View(coachToUpdate);
        }

        private void UpdateCoachActivities(string[] selectedActivities, Coach coachToUpdate)
        {
            if (selectedActivities == null)
            {
                coachToUpdate.Activities = new List<Activity>();
                return;
            }

            var selectedActivitiesHS = new HashSet<string>(selectedActivities);
            var coachActivities = new HashSet<int>
                (coachToUpdate.Activities.Select(c => c.ActivityID));

            foreach (var activity in db.Activities)
            {
                if (selectedActivitiesHS.Contains(activity.ActivityID.ToString()))
                {
                    if (!coachActivities.Contains(activity.ActivityID))
                    {
                        coachToUpdate.Activities.Add(activity);
                    }
                }
                else
                {
                    if (coachActivities.Contains(activity.ActivityID))
                    {
                        coachToUpdate.Activities.Remove(activity);
                    }
                }
            }
        }

        // GET: Coach/Delete/5
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Coach coach = db.Coachs.Include(c => c.FilePaths).SingleOrDefault(c => c.CoachID == id);
            
            if (coach == null)
            {
                return HttpNotFound();
            }
            return View(coach);
        }

        // POST: Coach/Delete/5
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Coach coach = db.Coachs
                .Where(c => c.CoachID == id)
                .Single();

            db.Coachs.Remove(coach);
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