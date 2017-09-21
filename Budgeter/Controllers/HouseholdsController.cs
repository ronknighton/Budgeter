using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Budgeter.Helpers;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UsersHelper usershelper = new UsersHelper();
        HouseholdsHelper householdsHelper = new HouseholdsHelper();

        // GET: Households
        [NoDirectAccess]
        public ActionResult Index()
        {
            var households = db.Households.Include(h => h.Members).ToList();
            return View(households);
        }

        [NoDirectAccess]
        public ActionResult MyHousehold()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = new Household();
            var budget = new Budget();
            if (user != null)
            {
                household = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == user.HouseholdId);
                ViewBag.HouseHold = household;
            }
            if(household == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (household != null)
            {
                budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id);
                ViewBag.Budget = budget;
            }            
           
            return View("Details", household);
        }

        // GET: Households/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Remove from household
        [NoDirectAccess]
        public ActionResult ListMembers(int hId)
        {
            var members = db.Users.Where(h => h.HouseholdId == hId).ToList();
            return View(members);
        }

        [NoDirectAccess]
        public ActionResult RemoveMember(string userId)
        {
            var user = db.Users.Find(userId);
            var household = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == user.HouseholdId);
            //householdsHelper.RemoveFromHousehold(user.Id, household.Id);
            ViewBag.UserId = user.Id;
            ViewBag.HouseholdId = household.Id;

            return View();
        }

        [NoDirectAccess]
        public ActionResult ConfirmRemoveMember(string userId, int householdId)
        {
            var currentUserId = User.Identity.GetUserId();

            householdsHelper.RemoveFromHousehold(userId, householdId);
            if(currentUserId == userId)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("ListMembers", new { hId = householdId });
        }

        [NoDirectAccess]
        public ActionResult AddMember(int hId)
        {
            //ViewBag.Email = email;
            ViewBag.hId = hId;
            //ViewBag.BackTo = backto;
            //ViewBag.Household = "User will be added to your household after you register them.";
            return RedirectToAction("AddMember", "Account", new {hId = hId });
        }

        [NoDirectAccess]
        public ActionResult CreateData()
        {
            TempData["CreateData"] = householdsHelper.CreatHouseholdData();
            return RedirectToAction("MyHousehold");
        }
        // GET: Households/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                household.Created = DateTimeOffset.Now;
                db.Households.Add(household);
                db.SaveChanges();

                var user = db.Users.Find(User.Identity.GetUserId());
                usershelper.AddToRole(user.Id, "HOH");
                user.Protected = true;
                user.EditBudget = true;
                user.EditHousehold = true;
                user.EditTransactions = true;
                user.AddBankAccounts = true;
                user.HouseholdId = household.Id;
                household.Members.Add(user);
                db.Entry(user).State = EntityState.Modified;
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = household.Id});
            }

            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Created")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyHousehold");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var household = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == id);

           
            
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == id);
            var bankAccounts = db.BankAccounts.Where(ba => ba.HouseholdId == id).ToList();
            if (bankAccounts.Count > 0)
            {
                foreach(var account in bankAccounts)
                {
                    db.BankAccounts.Remove(account);
                }
            }
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == id);
            if(budget != null)
            {
                var budgetItems = db.BudgetItems.Where(b => b.BudgetId == budget.Id).ToList();
                var categories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id).ToList();
                if(categories.Count > 0)
                {
                    foreach(var category in categories)
                    {
                        db.BudgetItemCategories.Remove(category);
                    }
                }
              
                db.SaveChanges();
               
            }
            foreach (var user in household.Members.ToList())
            {
                var userRoles = usershelper.ListUserRoles(user.Id);   
                foreach (var role in userRoles)
                {
                    usershelper.RemoveFromRole(user.Id, role);
                }  
                usershelper.AddToRole(user.Id, "Guest");
                user.HouseholdId = null;
                if (user.Protected)
                {
                    user.Protected = false;
                }
               
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            db.Households.Remove(household);            
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
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
