using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;

namespace Budgeter.Controllers
{
    //[RequireHttps]
    public class SampleBudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SampleBudgetItems
        public ActionResult Index()
        {
            var sampleBudgetItems = db.SampleBudgetItems.Include(s => s.Budget).Include(s => s.Category);
            return View(sampleBudgetItems.ToList());
        }

        // GET: SampleBudgetItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleBudgetItem sampleBudgetItem = db.SampleBudgetItems.Find(id);
            if (sampleBudgetItem == null)
            {
                return HttpNotFound();
            }
            return View(sampleBudgetItem);
        }

        // GET: SampleBudgetItems/Create
        public ActionResult Create()
        {
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name");
            return View();
        }

        // POST: SampleBudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Amount,BudgetId,CategoryId")] SampleBudgetItem sampleBudgetItem)
        {
            if (ModelState.IsValid)
            {
                db.SampleBudgetItems.Add(sampleBudgetItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", sampleBudgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name", sampleBudgetItem.CategoryId);
            return View(sampleBudgetItem);
        }

        // GET: SampleBudgetItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleBudgetItem sampleBudgetItem = db.SampleBudgetItems.Find(id);
            if (sampleBudgetItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", sampleBudgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name", sampleBudgetItem.CategoryId);
            return View(sampleBudgetItem);
        }

        // POST: SampleBudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Amount,BudgetId,CategoryId")] SampleBudgetItem sampleBudgetItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sampleBudgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", sampleBudgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudgetItemCategories, "Id", "Name", sampleBudgetItem.CategoryId);
            return View(sampleBudgetItem);
        }

        // GET: SampleBudgetItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleBudgetItem sampleBudgetItem = db.SampleBudgetItems.Find(id);
            if (sampleBudgetItem == null)
            {
                return HttpNotFound();
            }
            return View(sampleBudgetItem);
        }

        // POST: SampleBudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SampleBudgetItem sampleBudgetItem = db.SampleBudgetItems.Find(id);
            db.SampleBudgetItems.Remove(sampleBudgetItem);
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
