using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.Models
{
    //model with properties for a table with Username, RoleName and Edit column
    public class AdminUsersManageModel
    {
        public string RoleName { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
    }
    //we use this model to display user details
    public class AdminEditUserViewModel
    {
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        //public string UserLastName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
    }
    //we use this model to create userRole list
    public class AdminRoleViewModel
    {
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string RoleValue { get; set; }
    }
}