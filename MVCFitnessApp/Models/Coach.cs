using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.Models
{
    public class Coach
    {
        [Display(Name = "Number")]
        public int CoachID { get; set; }

        [Required]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        //Navigation properties. Holds entities (more than 1) related to a given entity.
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<FilePath> FilePaths { get; set; }
    }
}