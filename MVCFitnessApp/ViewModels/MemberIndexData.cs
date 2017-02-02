using MVCFitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.ViewModels
{
    public class MemberIndexData
    {
        public IEnumerable<Member> Members { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
    }
}