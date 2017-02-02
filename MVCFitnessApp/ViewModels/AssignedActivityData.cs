using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.ViewModels
{
    public class AssignedActivityData
    {
        public int ActivityID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}