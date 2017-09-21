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
    public class BudgetItemCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItemCategories
        [NoDirectAccess]
        public ActionResult Index()
        {
            return View(db.BudgetItemCategories.Include(b => b.BudgetItems).ToList());
        }

        [NoDirectAccess]
        public ActionResult _PartialCategoryIndex()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if(user != null && user.HouseholdId != null)
            {
                var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
                if (budget != null)
                {
                    var list = db.BudgetItemCategories.Include(b => b.BudgetItems).Where(b => b.BudgetId == budget.Id).ToList();
                    return PartialView(list);
                }
                
            }

            return PartialView(new List<BudgetItemCategory>());
        }
        // GET: BudgetItemCategories/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItemCategory budgetItemCategory = db.BudgetItemCategories.Include(b => b.BudgetItems).FirstOrDefault(b => b.Id == id);
            if (budgetItemCategory == null)
            {
                return HttpNotFound();
            }
            return View(budgetItemCategory);
        }

        // GET: BudgetItemCategories/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            return View();
        }

        [NoDirectAccess]
        public ActionResult _PartialCreateCategory()
        {
            return PartialView();
        }

        // POST: BudgetItemCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] BudgetItemCategory budgetItemCategory)
        {
            var budgetId = (int)TempData["BudgetId"];
            if (ModelState.IsValid)
            {
                budgetItemCategory.BudgetId = budgetId;
                db.BudgetItemCategories.Add(budgetItemCategory);
                db.SaveChanges();
                
                return RedirectToAction("Details", "Budgets", new { id = budgetId });
            }

           
            return RedirectToAction("Details", "Budgets", new { id = budgetId });
        }

        // GET: BudgetItemCategories/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItemCategory budgetItemCategory = db.BudgetItemCategories.Find(id);
            if (budgetItemCategory == null)
            {
                return HttpNotFound();
            }
            var budgetId = (int)TempData["BudgetId"];
            TempData["BudgetId"] = (int)TempData["BudgetId"];
            return RedirectToAction("Details", "Budgets", new { id = budgetId, editCatId = id });
        }

        [NoDirectAccess]
        public ActionResult _PartialEditCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItemCategory budgetItemCategory = db.BudgetItemCategories.Find(id);
            if (budgetItemCategory == null)
            {
                return HttpNotFound();
            }

            TempData["BudgetId"] = (int)TempData["BudgetId"];
            return PartialView(budgetItemCategory);
        }

        // POST: BudgetItemCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name, BudgetId")] BudgetItemCategory budgetItemCategory)
        {

            TempData["BudgetId"] = budgetItemCategory.BudgetId;
            if (ModelState.IsValid)
            {
                db.Entry(budgetItemCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Budgets", new { id = budgetItemCategory.BudgetId });
            }


            return RedirectToAction("Details", "Budgets", new { id = budgetItemCategory.BudgetId, editBiId = budgetItemCategory.Id });

        }

        // GET: BudgetItemCategories/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItemCategory budgetItemCategory = db.BudgetItemCategories.Include(b => b.BudgetItems).FirstOrDefault(b => b.Id == id);
            if (budgetItemCategory == null)
            {
                return HttpNotFound();
            }
            
            return View(budgetItemCategory);
        }

        // POST: BudgetItemCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItemCategory budgetItemCategory = db.BudgetItemCategories.Find(id);
            var budgetId = budgetItemCategory.BudgetId;
            db.BudgetItemCategories.Remove(budgetItemCategory);
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
