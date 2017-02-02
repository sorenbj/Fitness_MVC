using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MVCFitnessApp.Models
{
    public class Sponsor
    {
        public Sponsor()
        {
            this.Company = null;
            this.Website = null;
            this.Picture = null;
        }
        
        public Sponsor(int id, string company, string website, string picture)
        {
            this.Id = id;
            this.Company = company;
            this.Website = website;
            this.Picture = picture;
        }
        
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company name")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [Required]
        [Display(Name = "Logo file")]
        public string Picture { get; set; }

        public HttpPostedFileBase Logo { get; set; }


    }
}