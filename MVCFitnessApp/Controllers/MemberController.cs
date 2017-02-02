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
    public class MemberController : Controller
    {
        private Factory db = new Factory();

        // GET: Member
        public ActionResult Index(int? id, int? activityID, string searchString)
        {
            var viewModel = new MemberIndexData();
            viewModel.Members = db.Members
                .OrderBy(m => m.LastName);

            if (id != null)
            {           
                ViewBag.MemberID = id.Value;
                viewModel.Activities = viewModel.Members.Where(
                    m => m.MemberID == id.Value).Single().Activities;
            }

            return View(viewModel);
        }

        // GET: Member/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Member member = db.Members.Include(m => m.FilePathMembers).SingleOrDefault(m => m.MemberID == id);

            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: Member/Create
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Create()
        {
            var member = new Member();
            member.Activities = new List<Activity>();
            PopulateAssignedActivityData(member);
            return View();
        }

        // POST: Member/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberID,LastName,FirstName,EnrollmentDate")] Member member, string[] selectedActivities, HttpPostedFileBase upload)
        {
            if (selectedActivities != null)
            {
                member.Activities = new List<Activity>();
                foreach (var activity in selectedActivities)
                {
                    var activityToAdd = db.Activities.Find(int.Parse(activity));
                    member.Activities.Add(activityToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var photo = new FilePathMember
                    {
                        FileName = Path.GetFileName(upload.FileName),
                        FileType = "Photo"
                    };

                    string path = Path.Combine(Server.MapPath("~/Pics/Member/"), Path.GetFileName(upload.FileName));

                    upload.SaveAs(path);
                    member.FilePathMembers = new List<FilePathMember>();
                    member.FilePathMembers.Add(photo);
                }

                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssignedActivityData(member);
            return View(member);
        }

        // GET: Member/Edit/5
        [Authorize(Roles = "User, Coach, Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Member member = db.Members
                .Include(m => m.Activities)
                .Where(m => m.MemberID == id)
                .Single();

            PopulateAssignedActivityData(member);

            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Member/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "User, Coach, Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedActivities, HttpPostedFileBase upload)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var memberToUpdate = db.Members
               .Include(m => m.Activities)
               .Where(m => m.MemberID == id)
               .Include(m => m.FilePathMembers)
               .Single();

            if (TryUpdateModel(memberToUpdate, "",
               new string[] { "LastName", "FirstName", "EnrollmentDate" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (memberToUpdate.FilePathMembers.Any(f => f.FileType == "Photo"))
                        {
                            db.FilePathMembers.Remove(memberToUpdate.FilePathMembers.First(f => f.FileType == "Photo"));
                        }

                        var photo = new FilePathMember
                        {
                            FileName = Path.GetFileName(upload.FileName),
                            FileType = "Photo",
                        };

                        string path = Path.Combine(Server.MapPath("~/Pics/Member/"), Path.GetFileName(upload.FileName));

                        upload.SaveAs(path);
                        memberToUpdate.FilePathMembers = new List<FilePathMember> { photo };
                    }

                    UpdateMemberActivities(selectedActivities, memberToUpdate);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedActivityData(memberToUpdate);
            return View(memberToUpdate);
        }

        private void UpdateMemberActivities(string[] selectedActivities, Member memberToUpdate)
        {
            if (selectedActivities == null)
            {
                memberToUpdate.Activities = new List<Activity>();
                return;
            }

            var selectedActivitiesHS = new HashSet<string>(selectedActivities);
            var memberActivities = new HashSet<int>
                (memberToUpdate.Activities.Select(m => m.ActivityID));

            foreach (var activity in db.Activities)
            {
                if (selectedActivitiesHS.Contains(activity.ActivityID.ToString()))
                {
                    if (!memberActivities.Contains(activity.ActivityID))
                    {
                        memberToUpdate.Activities.Add(activity);
                    }
                }
                else
                {
                    if (memberActivities.Contains(activity.ActivityID))
                    {
                        memberToUpdate.Activities.Remove(activity);
                    }
                }
            }
        }

        // GET: Member/Delete/5
        [Authorize(Roles = "Coach, Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Member member = db.Members.Include(m => m.FilePathMembers).SingleOrDefault(m => m.MemberID == id);

            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Member/Delete/5
        [Authorize(Roles = "Coach, Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members
                .Where(m => m.MemberID == id)
                .Single();

            db.Members.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateAssignedActivityData(Member member)
        {
            var allActivities = db.Activities;
            var memberActivities = new HashSet<int>(member.Activities.Select(e => e.ActivityID));
            var viewModel = new List<AssignedActivityData>();

            foreach (var activity in allActivities)
            {
                //viewmodel to get ActivityID and Name from Activity
                viewModel.Add(new AssignedActivityData
                {
                    ActivityID = activity.ActivityID,
                    Name = activity.Name,
                    Assigned = memberActivities.Contains(activity.ActivityID)
                });
            }
            ViewBag.Activities = viewModel;
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
