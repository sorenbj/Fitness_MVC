using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.Models
{
    public class Activity
    {
        //Primary Key
        [Display(Name = "Number")]
        public int ActivityID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Picture")]
        public string PictureFilename { get; set; }

        //Navigation properties. Holds entities (more than 1) related to a given entity.
        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<Coach> Coachs { get; set; }
    }
}