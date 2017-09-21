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

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        [NoDirectAccess]
        public ActionResult Index()
        {
            var budgetItems = db.BudgetItems.Include(b => b.Budget).Include(b => b.Category).Where(b => b.Active);
            return View(budgetItems.ToList());
        }

        [NoDirectAccess]
        public ActionResult _PartialBudgetItemsIndex()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user != null && user.HouseholdId != null)
            {
                var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
                if (budget != null)
                {
                    var budgetItems = db.BudgetItems.Include(b => b.Budget).Include(b => b.Category).Where(b => b.BudgetId == budget.Id && b.Active);
                     return PartialView(budgetItems.ToList());
                }
            }
            return PartialView(new List<BudgetItem>());
           
        }
        // GET: BudgetItems/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // GET: BudgetItems/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.hId = user.HouseholdId;
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name");
            return View();
        }

        [NoDirectAccess]
        public ActionResult _PartialCreateBudgetItems()
        {
            var budgetId = (int)TempData["BudgetId"];
            TempData["BudgetId"] = (int)TempData["BudgetId"];
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.hId = user.HouseholdId;
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
            //ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories.Where(bi => bi.BudgetId == budgetId).ToList(), "Id", "Name");
            return PartialView();
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Amount,CategoryId")] BudgetItem budgetItem)
        {
            var budgetId = (int)TempData["BudgetId"];
            if (ModelState.IsValid)
            {
                budgetItem.Active = true;
                budgetItem.BudgetId = budgetId;
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Details", "Budgets", new { id = budgetId });
            }

            TempData["BudgetId"] = budgetId; 
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name", budgetItem.CategoryId);
            return RedirectToAction("Details", "Budgets", new { id = budgetId });
        }

        // GET: BudgetItems/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name", budgetItem.CategoryId);


            var budgetId = budgetItem.BudgetId;
            TempData["BudgetId"] = budgetId;
            return RedirectToAction("Details", "Budgets", new { id = budgetId, editBiId = id });
        }

        [NoDirectAccess]
        public ActionResult _PartialEditBudgetItems(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            TempData["BudgetId"] = budgetItem.BudgetId;
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories.Where(bi => bi.BudgetId == budgetItem.BudgetId).ToList(), "Id", "Name", budgetItem.CategoryId);

            return PartialView(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Active, Amount,BudgetId,CategoryId")] BudgetItem budgetItem)
        {
            var budgetId = budgetItem.BudgetId;
            if (ModelState.IsValid)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                
                return RedirectToAction("Details", "Budgets", new { id = budgetId});
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name", budgetItem.CategoryId);
            return RedirectToAction("Details", "Budgets", new { id = budgetId, editBiId = budgetItem.Id});
        }

        // GET: BudgetItems/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);

            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            var budgetId = budgetItem.BudgetId;
            var transactions = db.Transactions.Where(t => t.BudgetItemId == id).ToList();
            foreach(var t in transactions)
            {
                t.BudgetItemId = null;
                db.Entry(t).State = EntityState.Modified;
            }
            db.BudgetItems.Remove(budgetItem);
            db.SaveChanges();
            
            return RedirectToAction("Details", "Budgets", new { id = budgetId });
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
