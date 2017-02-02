using MVCFitnessApp.Models;
using MVCFitnessApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCFitnessApp.Controllers
{
    public class HomeController : Controller
    {
        private Factory db = new Factory();

        public ActionResult Index()
        {
            var factory = new Factory();
            ViewBag.activities = factory.Activities.ToList();
            var repo = new SponsorRepo();
            ViewBag.sponsers = repo.GetSponsors();
            
            return View();
        }

        public ActionResult About()
        {
            IQueryable<EnrollmentDateGroup> data = from member in db.Members
                                                   group member by member.EnrollmentDate into dateGroup
                                                   select new EnrollmentDateGroup()
                                                   {
                                                       EnrollmentDate = dateGroup.Key,
                                                       MemberCount = dateGroup.Count()
                                                   };
            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact your local fitness center";

            return View();
        }

        public ActionResult Details(int id)
        {
            var factory = new Factory();
            var found = factory.Activities.Where(p => p.ActivityID == id).FirstOrDefault();
            return View(found);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}