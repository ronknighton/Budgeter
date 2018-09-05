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
    //[RequireHttps]
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private BudgetsHelper budgetsHelper = new BudgetsHelper();

        // GET: Budgets
        [NoDirectAccess]
        public ActionResult Index()
        {
            var budgets = db.Budgets.Include(b => b.Household);
            return View(budgets.ToList());
        }

        // GET: Budgets/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id, int? editCatId, int? editBiId)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var user = db.Users.Find(User.Identity.GetUserId());
                ViewBag.hId = user.HouseholdId;

                ViewBag.BudgetItems = new MultiSelectList(db.SampleBudgetItems, "Id", "Name");
                return View("Create");
            }
            Budget budget = db.Budgets.Include(b => b.BudgetItems).FirstOrDefault(b => b.Id == id);
            if (budget == null)
            {
                //return HttpNotFound();
                var user = db.Users.Find(User.Identity.GetUserId());
                ViewBag.hId = user.HouseholdId;

                ViewBag.BudgetItems = new MultiSelectList(db.SampleBudgetItems, "Id", "Name");
                return View("Create");
            }
            TempData["BudgetId"] = id;
            ViewBag.EditCategoryId = editCatId;
            ViewBag.EditItemId = editBiId;

            return View(budget);
        }

        // GET: Budgets/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.hId = user.HouseholdId;

            ViewBag.BudgetItems = new MultiSelectList(db.SampleBudgetItems, "Id", "Name");
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Amount")] Budget budget, List<int> BudgetItems)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var sampleCategories = db.SampleBudgetItemCategories.AsNoTracking().ToList();
                budget.HouseholdId = (int)user.HouseholdId;
                budget.Created = DateTimeOffset.Now;
                budget.Active = true;
                db.Budgets.Add(budget);
                db.SaveChanges();

                foreach (var c in sampleCategories)
                {
                    var category = new BudgetItemCategory();
                    category.Name = c.Name;
                    category.BudgetId = budget.Id;
                    db.BudgetItemCategories.Add(category);
                }
                db.SaveChanges();

                BudgetItem budgetItem;
                SampleBudgetItem sampleBudgetItem;
                SampleBudgetItemCategory sampleCategory;

                foreach (var bid in BudgetItems)
                {
                    budgetItem = new BudgetItem();
                    sampleBudgetItem = db.SampleBudgetItems.AsNoTracking().FirstOrDefault(b => b.Id == bid);
                    sampleCategory = db.SampleBudgetItemCategories.FirstOrDefault(c => c.Id == sampleBudgetItem.CategoryId);
                    budgetItem.Name = sampleBudgetItem.Name;
                    budgetItem.Amount = sampleBudgetItem.Amount;
                    budgetItem.CategoryId = db.BudgetItemCategories.FirstOrDefault(c => c.Name == sampleCategory.Name && c.BudgetId == budget.Id).Id;
                    budgetItem.BudgetId = budget.Id;
                    budgetItem.Active = true;
                    budget.BudgetItems.Add(budgetItem);
                    db.BudgetItems.Add(budgetItem);
                    db.Entry(budget).State = EntityState.Modified;
                    //db.SaveChanges();
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = budget.Id });
            }

            ViewBag.BudgetItems = new MultiSelectList(db.BudgetItems, "Id", "Name");
            return View(budget);
        }

        // GET: Budgets/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            var currentItems = budget.BudgetItems.Select(d => d.Id).ToList();
            ViewBag.BudgetItems = new MultiSelectList(db.BudgetItems.Where(b => b.BudgetId == budget.Id && b.Active).ToList(), "Id", "Name", currentItems);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Active,Name,Created,Amount,HouseholdId")] Budget budget, List<int> BudgetItems)
        {
            if (ModelState.IsValid)
            {

                foreach (var item in budgetsHelper.ListBudgetItems(budget.Id))
                {
                    budgetsHelper.RemoveBudgetItem(budget.Id, item.Id);
                }

                foreach (var bId in BudgetItems)
                {
                    budgetsHelper.AddBudgetItem(budget.Id, bId);
                }

                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = budget.Id });
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            var currentItems = budget.BudgetItems.Select(d => budget.Id);
            ViewBag.BudgetItems = new MultiSelectList(db.BudgetItems, "Id", "Name", currentItems);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Include(b => b.BudgetItems).FirstOrDefault(b => b.Id == id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            var budgetItems = db.BudgetItems.Where(b => b.BudgetId == id).ToList();
            var categories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id).ToList();

            foreach (var item in budgetItems)
            {
                //item.BudgetId = null;
                //db.Entry(item).State = EntityState.Modified;
                var transactions = db.Transactions.Where(t => t.BudgetItemId == item.Id).ToList();
                foreach (var t in transactions)
                {
                    t.BudgetItemId = null;
                    db.Entry(t).State = EntityState.Modified;

                }
                db.BudgetItems.Remove(item);

            }
            foreach (var c in categories)
            {
                db.BudgetItemCategories.Remove(c);
            }
            db.Budgets.Remove(budget);
            db.SaveChanges();
            return RedirectToAction("MyHousehold", "Households");
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
