using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class UsersHelper
    {
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();

        public string UserProfileImage()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                if (user != null && user.DisplayImage != null)
                {
                    return user.DisplayImage;
                }
            }
            return "/Assets/images/profile-pics/defaultProfileImage.jpg";


        }

        public string getUserDisplayImage(string userId)
        {
            var user = db.Users.Find(userId);
            if (user != null && user.DisplayImage != null)
            {
                return user.DisplayImage;
            }

            return "/Assets/images/profile-pics/einstein.jpg";
        }

        public string GetUserNameFromId(string userId)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return "MMS Messaging";
            }

            return user.FullName;
        }

        public bool UserHasEverything()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user != null && user.HouseholdId != null)
            {
                var hId = user.HouseholdId;
                var budgets = db.Budgets.Where(b => b.HouseholdId == hId).ToList().Count();

                if (budgets > 0)
                {
                    var bankAccounts = db.BankAccounts.Where(b => b.HouseholdId == hId).ToList().Count();

                    if (bankAccounts > 0)
                    {
                        return true;
                    }
                }


            }
            return false;
        }


        public bool HaveHousehold()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user == null)
            {
                return false;
            }
            if (user.HouseholdId == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool HaveBudget()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user != null && user.HouseholdId != null)
            {
                var budgets = db.Budgets.Where(b => b.HouseholdId == user.HouseholdId).ToList().Count();

                if (budgets > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsAccountOwner(int id)
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if(user != null)
            {
                var account = db.BankAccounts.FirstOrDefault(a => a.Id == id && a.AccountOwnerId == user.Id);
                if (account != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HaveBankAccount()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user != null && user.HouseholdId != null)
            {
                var bankAccounts = db.BankAccounts.Where(b => b.HouseholdId == user.HouseholdId).ToList().Count();
                if(bankAccounts > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void ProtectUser(string userId)
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                user.Protected = true;
                db.Entry(user).State = EntityState.Modified;
            }
        }

        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }

        public bool IsUserInRoleByUsername(string userName, string role)
        {
            var user = userManager.FindByEmail(userName);
            if (user != null)
            {
                return IsUserInRole(user.Id, role);
            }
            else
            {
                return false;
            }
        }

        public bool IsHOH(string userName)
        {
            return IsUserInRoleByUsername(userName, "HOH");
        }

        public bool IsHOH()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());

            return IsUserInRole(user.Id, "HOH");
        }

        public bool IsCurrentUser(string userId)
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (userId == user.Id)
            {
                return true;
            }
            return false;
        }
        //Add a user to a role and return true if success
        public bool AddToRole(string userId, string roleName)
        {
            foreach (var role in ListUserRoles(userId))
            {
                RemoveFromRole(userId, role);
            }
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        //Remove user form role and return tru if successful
        public bool RemoveFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public ICollection<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public string GetUserRole(string userId)
        {
            var role = userManager.GetRoles(userId).ToList()[0];
            //return db.Roles.Select(r => r.Name).Where(r => r == "HOH" || r == "Member").ToList()[0];
            return role;
        }

        public bool AddBankAccounts()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            return user.AddBankAccounts;
        }
        public bool EditHousehold()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            return user.EditHousehold;
        }
        public bool EditBudget()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            return user.EditBudget;
        }
        public bool EditTransactions()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            return user.EditTransactions;
        }
    }
}