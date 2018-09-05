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
    //[RequireHttps]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        [NoDirectAccess]
        public ActionResult Index()
        {
            var bankAccounts = db.BankAccounts.Include(b => b.AccountOwner).Include(b => b.AccountType).Include(b => b.Household);
            return View(bankAccounts.ToList());
        }

        [NoDirectAccess]
        // GET: User BankAccounts
        public ActionResult UserIndex()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var bankAccounts = db.BankAccounts.Include(b => b.AccountOwner).Include(b => b.AccountType).Include(b => b.Household).Where(b => b.HouseholdId == user.HouseholdId).ToList();
            return View("Index",bankAccounts);
        }

        // GET: BankAccounts/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.hId = user.HouseholdId;
            TempData["hId"] = user.HouseholdId;
            ViewBag.AccountOwnerId = new SelectList(db.Users.Where(u => u.HouseholdId == user.HouseholdId).ToList(), "Id", "FirstName");
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type");
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Balance,WarningBalance,BankName,AccountNumber,AccountTypeId,AccountOwnerId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                if(db.BankAccounts.Any(b => b.Name == bankAccount.Name))
                {
                    ViewBag.AccountName = "Another account already exists with that name";
                    ViewBag.AccountOwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.AccountOwnerId);
                    ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", bankAccount.AccountTypeId);
                    ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
                    return View(bankAccount);
                }
              if(bankAccount.AccountNumber != null && db.BankAccounts.Any(b => b.AccountNumber == bankAccount.AccountNumber))
                {
                    ViewBag.AccountNumber = "Another account already exists with that account number";
                    ViewBag.AccountOwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.AccountOwnerId);
                    ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", bankAccount.AccountTypeId);
                    ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
                    return View(bankAccount);
                }
                bankAccount.HouseholdId = (int)TempData["hId"];
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("UserIndex");
            }

            ViewBag.AccountOwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.AccountOwnerId);
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", bankAccount.AccountTypeId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
             ViewBag.AccountOwnerId = new SelectList(db.Users.Where(u => u.HouseholdId == bankAccount.HouseholdId).ToList(), "Id", "FirstName");;
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", bankAccount.AccountTypeId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Balance,WarningBalance,BankName,AccountNumber,HouseholdId,AccountTypeId,AccountOwnerId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                if (db.BankAccounts.Any(b => b.Name == bankAccount.Name && b.Id != bankAccount.Id))
                {
                    ViewBag.AccountName = "Another account already exists with that name";
                    ViewBag.AccountOwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.AccountOwnerId);
                    ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", bankAccount.AccountTypeId);
                    ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
                    return View(bankAccount);
                }
                if (bankAccount.AccountNumber != null && db.BankAccounts.Any(b => b.AccountNumber == bankAccount.AccountNumber && b.Id != bankAccount.Id))
                {
                    ViewBag.AccountNumber = "Another account already exists with that account number";
                    ViewBag.AccountOwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.AccountOwnerId);
                    ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", bankAccount.AccountTypeId);
                    ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
                    return View(bankAccount);
                }
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserIndex");
            }
            ViewBag.AccountOwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.AccountOwnerId);
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", bankAccount.AccountTypeId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
            db.SaveChanges();
            return RedirectToAction("UserIndex");
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
