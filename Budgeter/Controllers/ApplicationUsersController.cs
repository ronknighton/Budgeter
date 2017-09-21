using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Budgeter.Helpers;
using Microsoft.AspNet.Identity;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UsersHelper usersHelper = new UsersHelper();

        // GET: ApplicationUsers
        [NoDirectAccess]
        public ActionResult Index()
        {
            var applicationUsers = db.Users.Include(a => a.Household);
            return View(applicationUsers.ToList());
        }

        [NoDirectAccess]
        public ActionResult MembersIndex(int hId)
        {
            var members = db.Users.Where(u => u.HouseholdId == hId).ToList();

            return View(members);
        }

        //GET
        [NoDirectAccess]
        public ActionResult ProfileChanges(string userId)
        {
            ApplicationUser applicationUser = db.Users.Find(userId);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        [HttpPost]
        public ActionResult ProfileChanges(string userId, string FullName, HttpPostedFileBase image)
        {
            var user = db.Users.Find(userId);

            //if (user.Protected == true)
            //{
            //    ViewBag.AdminError = "Can't change protected user.";
            //    return View(user);
            //}

            #region Check Image
            if (image != null)
            {
                var fileValidator = new FileUploadValidator();
                var fileUrl = fileValidator.ValidateAndSaveFile3(image);
                var message = "";

                switch (fileUrl)
                {
                    case "null":
                        message = "An Image Must Be Selected";
                        break;
                    case "small":
                        message = "Invalid Image: Must be larger than 3Kb.";
                        break;
                    case "large":
                        message = "Invalid Image: Must be smaller than 2Mb.";
                        break;
                    case "format":
                        message = "Invalid Image format";
                        break;
                }

                if (message != "")
                {
                    TempData["FileError"] = message;
                    ApplicationUser applicationUser = db.Users.Find(userId);
                    if (applicationUser == null)
                    {
                        return HttpNotFound();
                    }
                    return View(applicationUser);

                }

                user.DisplayImage = fileUrl;
            }
            #endregion
            if (!string.IsNullOrWhiteSpace(FullName))
            {
                if (FullName.Trim().Length < 5)
                {
                    TempData["DisplayNameError"] = "Display name must be greater than 5 characters";
                    ApplicationUser applicationUser = db.Users.Find(User.Identity.GetUserId());
                    if (applicationUser == null)
                    {
                        return HttpNotFound();
                    }
                    return View(applicationUser);
                }

                user.FullName = FullName;
            }

            //db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            if (user.HouseholdId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("ListMembers", "Households", new { hId = user.HouseholdId });
        }
        //GET Change user role
        [NoDirectAccess]
        public ActionResult ChangeRole(string userId)
        {

            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }


            var currentRoles = usersHelper.ListUserRoles(user.Id);
            var currentRole = usersHelper.GetUserRole(user.Id);

            var UserRoles = db.Roles.Select(r => r.Name).Where(r => r == "HOH" || r == "Member").ToList();

            ViewBag.userRoles = new SelectList(UserRoles, currentRole);
            ViewBag.EditBudget = user.EditBudget;
            ViewBag.AddBankAccount = user.AddBankAccounts;
            ViewBag.EditHousehold = user.EditHousehold;
            ViewBag.EditTransactions = user.EditTransactions;

            return View(user);

        }

        [HttpPost]
        public ActionResult ChangeRole(string userId, string userRoles, bool EditBudget, bool AddBankAccounts, bool EditHousehold, bool EditTransactions)
        {
            var user = db.Users.Find(userId);
            if (userRoles == "HOH")
            {
                user.AddBankAccounts = true;
                user.EditBudget = true;
                user.EditHousehold = true;
                user.EditTransactions = true;
            }
            else
            {
                user.AddBankAccounts = AddBankAccounts;
                user.EditBudget = EditBudget;
                user.EditHousehold = EditHousehold;
                user.EditTransactions = EditTransactions;
            }


            //var role = db.Roles.FirstOrDefault(r => r.Id == userRoles).Name;
            usersHelper.AddToRole(user.Id, userRoles);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListMembers", "Households", new { hId = user.HouseholdId });

        }

        // GET: ApplicationUsers/Details/5
        [NoDirectAccess]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,FullName,Protected,DisplayImage,HouseholdId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", applicationUser.HouseholdId);
            return View(applicationUser);
        }

        // GET: Users/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", applicationUser.HouseholdId);
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,FullName,Protected,DisplayImage,HouseholdId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", applicationUser.HouseholdId);
            return View(applicationUser);
        }

        // GET: Users/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
