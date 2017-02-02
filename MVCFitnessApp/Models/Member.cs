using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.Models
{
    public class Member
    {
        //Public key
        [Display(Name = "Number")]
        public int MemberID { get; set; }  

        [Required]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        //Navigation property. Holds entities (more than 1) related to a given entity.
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<FilePathMember> FilePathMembers { get; set; }
    }
}