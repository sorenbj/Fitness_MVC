using MVCFitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MVCFitnessApp.Controllers
{
    [AllowAnonymous] // Needs to be changed to Administrator
    //[Authorize(Roles ="Administrator")]
    public class SponsorController : Controller
    {
        private SponsorRepo repo = new SponsorRepo();


        // GET: Sponsors
        public ActionResult Index()        // WORKING <-------
        {
            return View(repo.GetSponsors());
        }


        // GET: Sponsor/Details/5
        public ActionResult Details(int id)
        {
            Sponsor sponsor = repo.GetSponsorById(id);
            if(sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }


        // GET: Sponsor/Create
        public ActionResult Create()        // WORKING <-------
        {
            return View();
        }

        // POST: Sponsor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sponsor sponsor)        // WORKING <-------
        {
            if (ModelState.IsValid)
            {
                if (sponsor.Logo != null)
                {
                    sponsor.Logo.SaveAs(HttpContext.Server.MapPath("~/Pics/sponsor/") + sponsor.Logo.FileName);
                }

                var SponserData = new Sponsor()
                {
                    Company = sponsor.Company,
                    Website = sponsor.Website,
                    Picture = sponsor.Logo.FileName
                };

                repo.InsertSponsor(SponserData);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Sponsor/Edit/5
        public ActionResult Edit(int id)        // WORKING <-------
        {
            Sponsor sponsor = repo.GetSponsorById(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        // POST: Sponsor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sponsor sponsor)        // WORKING <-------
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    repo.EditSponsor(sponsor);
                    return View(sponsor);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Sponsor/Delete/5
        public ActionResult Delete(int id)
        {
            // Get the sponsor from repository by id
            Sponsor sponsor = repo.GetSponsorById(id);
            // Check retrieved sponsor object
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            // Return delete view for this sponsor
            return View(sponsor);
        }

        // POST: Sponsor/Delete/5
        [HttpPost]
        public ActionResult Delete(Sponsor sponsor)
        {
            // Delete logic here
            try
            {
                // Get this sponsor id as id
                int id = sponsor.Id;
                // Call the delete method in repository with id as parametre
                repo.DeleteSponsor(id);
                // Then redirect to the sponsor index page
                return RedirectToAction("Index");
            }
            catch
            {
                return View(sponsor);
            }
        }
    }
}
