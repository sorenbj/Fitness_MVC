using MVCFitnessApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCFitnessApp.Controllers
{
    public class AdministratorController : Controller
    {
        #region vars
        //we needUserManager for user related action
        public UserManager<ApplicationUser> UserManager { get; set; }
        //DbContext object to access database
        public ApplicationDbContext context { get; set; }

        //2 list for users and roles
        public static List<AdminUsersManageModel> usersList = new List<AdminUsersManageModel>();
        public static List<SelectListItem> roleList = new List<SelectListItem>();

        //properties to hold values we will need
        public static string AdmnUserName { get; set; }
        public static string AdmnUserRole { get; set; }
        public static string AdmnUserEmail { get; set; }
        public static string AdmnRoleSrch { get; set; }
        public static string AdmnNameSrch { get; set; }
        #endregion



        public AdministratorController()
        {
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }

        //Index method
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult> Index(AdminUsersManageModel model, ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.UserDelete ? "User was DELETED"
                : message == ManageMessageId.UserUpdate ? "User was Updated"
                : "";

            ViewBag.ErrorMessage =
                message == ManageMessageId.Error ? "An error has accured"
                : message == ManageMessageId.Error ? "Cannot delete high rank user"
                : "";

            //we call ShowUsers.. method 
            await ShowUsersDetails(model);
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [ActionName("Index")]
        public async Task<ActionResult> ShowUsersDetails(AdminUsersManageModel model)
        {
            //we clear the list to prevent duplications
            usersList.Clear();

            //we create the list of users
            IList<ApplicationUser> users = context.Users.ToList();
           
            foreach (var user in users)
            {
                //for each user we get name and role
                var roles = await UserManager.GetRolesAsync(user.Id);
                model.UserName = user.UserName;

                foreach (var role in roles)
                {
                    model.RoleName = role;
                    switch (role)
                    {
                        case "Administrator":
                            model.RoleId = "1";
                            break;
                        case "Coach":
                            model.RoleId = "2";
                            break;
                        case "User":
                            model.RoleId = "3";
                            break;
                    }
                }
                //we get users id and name
                model.UserId = user.Id;
                model.UserFullName = user.FirstName + " " + user.LastName;

                //we add user to the list with its properties
                usersList.Add(new AdminUsersManageModel() {
                    UserName = model.UserName,
                    RoleName = model.RoleName,
                    RoleId = model.RoleId,
                    UserId = model.UserId,
                    UserFullName = model.UserFullName
                });
                model.RoleName = null;
            }

            return PartialView("ShowUsersDetails");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveUser(string id, AdminEditUserViewModel model)
        {
            try
            {
                AdmnUserRole = model.RoleName;
                AdmnUserName = model.UserName;

                //we find the user's ID by search the database with username
                var userid = context.Users.Where(x => x.UserName == AdmnUserName).Select(x => x.Id).FirstOrDefault();

                //with the user id we get user roles and create a string array of roles
                var user = await UserManager.FindByIdAsync(userid);
                var userRole = await UserManager.GetRolesAsync(user.Id);
                string[] roles = new string[userRole.Count];
                userRole.CopyTo(roles, 0);

                //we remove any existing roles of the user
                await UserManager.RemoveFromRolesAsync(user.Id, roles);

                //we asign new role
                await UserManager.AddToRoleAsync(user.Id, AdmnUserRole);

                return RedirectToAction("Index", "Administrator", new { Message = ManageMessageId.UserUpdate });
            }
            catch
            {
                return RedirectToAction("Index", "Administrator", new { Message = ManageMessageId.Error });
            }
        }

        public enum ManageMessageId
        {
            HighRankedUser,
            Error,
            UserDelete,
            UserUpdate
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult DeleteUser()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(string userid)
        {
            //we dont allow the admin to delete other admins
            if(AdmnUserRole == "Administrator")
            {
                return RedirectToAction("Index", "Administrator", new { Message = ManageMessageId.HighRankedUser });
            }
            //to delete the user we have to delete all claims, roles,external logins related with that user
            userid = context.Users.Where(x => x.UserName == AdmnUserName).Select(x => x.Id).FirstOrDefault();
            var user = await UserManager.FindByIdAsync(userid);
            var userClaims = await UserManager.GetClaimsAsync(user.Id);
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var userLogins = await UserManager.GetLoginsAsync(user.Id);
            foreach(var claim in userClaims)
            {
                await UserManager.RemoveClaimAsync(user.Id, claim);
            }
            //string[] roles = new string[userRoles.Count];
            //userRoles.CopyTo(roles, 0);
            //await UserManager.RemoveFromRoleAsync(user.Id, roles);
            string role;
            role = userRoles[0];
            await UserManager.RemoveFromRoleAsync(user.Id, role);
            foreach (var log in userLogins)
            {
                await UserManager.RemoveLoginAsync(user.Id, new UserLoginInfo(log.LoginProvider, log.ProviderKey));
            }
            await UserManager.DeleteAsync(user);

            return RedirectToAction("Index", "Administrator", new { Message = ManageMessageId.UserDelete});
        }

        //var user = await UserManager.FindByIdAsync(userid);
        //var userRole = await UserManager.GetRolesAsync(user.Id);
        //string[] roles = new string[userRole.Count];
        //userRole.CopyTo(roles, 0);

        //        //we remove any existing roles of the user
        //        await UserManager.RemoveFromRolesAsync(user.Id, roles);

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult EditUser()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(string id, AdminEditUserViewModel model)
        {
            try
            {
                //find the user by id and pass it to edit column
                var user = UserManager.FindById(id);
                //with the user found we get its the details
                model.Email = user.Email;
                var roles = await UserManager.GetRolesAsync(user.Id);
                model.UserName = user.UserName;
                foreach (var role in roles)
                {
                    model.RoleName = role;
                }

                AdmnUserName = model.UserName;
                AdmnUserEmail = model.Email;
                AdmnUserRole = model.RoleName;
                return RedirectToAction("EditUser");
            }
            catch
            {
                return View();
            }
        }

        public IEnumerable<SelectListItem> GetUserRoles(string usrrole)
        {
            //we create our role list for our edit view
            var roles = context.Roles.OrderBy(x => x.Name).ToList();
            List<AdminRoleViewModel> rList = new List<AdminRoleViewModel>();
            rList.Add(new AdminRoleViewModel() { RoleName = "Administrator", RoleId = "1" });
            rList.Add(new AdminRoleViewModel() { RoleName = "Coach", RoleId = "2" });
            rList.Add(new AdminRoleViewModel() { RoleName = "User", RoleId = "3" });
            rList = rList.OrderBy(x => x.RoleId).ToList();

            List<SelectListItem> roleNames = new List<SelectListItem>();
            foreach (var role in rList)
            {
                roleNames.Add(new SelectListItem()
                {
                    Text = role.RoleName,
                    Value = role.RoleName
                });
            }
            //we select our users role from a dropdown list
            var SelectedRoleName = roleNames.FirstOrDefault(d => d.Value == usrrole);
            if(SelectedRoleName != null)           
                SelectedRoleName.Selected = true;
            return roleNames;
         }


      }
   }
