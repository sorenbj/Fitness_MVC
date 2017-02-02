using MVCFitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.ViewModels
{
    public class CoachIndexData
    {
        public IEnumerable<Coach> Coachs { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public IEnumerable<Member> Members { get; set; }
    }
}