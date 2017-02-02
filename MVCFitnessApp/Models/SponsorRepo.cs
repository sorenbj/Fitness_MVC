 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace MVCFitnessApp.Models
{
    public class SponsorRepo: ISponsorRepo
    {
        private List<Sponsor> sponsorList;
        private XDocument sponsorData;
        
        // Constructor
        public SponsorRepo()
        {
            sponsorList = new List<Sponsor>();

            sponsorData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/Sponsors.xml")); //Path of the xml script
            var xmlsponsors = from sponsors in sponsorData.Descendants("sponsor")
                           select new Sponsor((int)sponsors.Element("id"), sponsors.Element("company").Value,
                           sponsors.Element("website").Value, sponsors.Element("picture").Value);
            sponsorList.AddRange(xmlsponsors.ToList<Sponsor>());
        }

        // Return a list of all sponsors from xml file
        public IEnumerable<Sponsor> GetSponsors()
        {
            return sponsorList;
        }

        // Return sponsor by Id
        public Sponsor GetSponsorById(int id)
        {
            return sponsorList.Find(sponsor => sponsor.Id == id);
        }

        // Insert Sponsor
        public void InsertSponsor(Sponsor sponsor)
        {
            sponsor.Id = (int)(from sp in sponsorData.Descendants("sponsor")
                               orderby (int)sp.Element("id") descending
                               select (int)sp.Element("id")).FirstOrDefault() + 1;

            sponsorData.Root.Add(new XElement("sponsor", 
                new XElement("id", sponsor.Id), 
                new XElement("company", sponsor.Company),
                new XElement("website", sponsor.Website), 
                new XElement("picture", sponsor.Picture)));

            sponsorData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Sponsors.xml"));
        }

        // Delete Sponsor
        public void DeleteSponsor(int id)
        {
            sponsorData.Root.Elements("sponsor").Where(i => (int)i.Element("id") == id).Remove();

            sponsorData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Sponsors.xml"));
        }

        // Edit Sponsor
        public void EditSponsor(Sponsor sponsor)
        {
            XElement node = sponsorData.Root.Elements("sponsor").Where(i => (int)i.Element("id") == sponsor.Id).FirstOrDefault();

            node.SetElementValue("company", sponsor.Company);
            node.SetElementValue("website", sponsor.Website);
            node.SetElementValue("picture", sponsor.Picture);

            sponsorData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Sponsors.xml"));
        }

    }
}