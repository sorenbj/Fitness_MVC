using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.ViewModels
{
    public class EnrollmentDateGroup
    {
        // About page - count members enrolled sorted by date
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }

        public int MemberCount { get; set; }
    }
}