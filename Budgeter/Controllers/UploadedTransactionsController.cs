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
using System.Threading.Tasks;

namespace Budgeter.Controllers
{
    public class UploadedTransactionsController : Controller
    {
        private CsvHelper csvHelper = new CsvHelper();
        private ApplicationDbContext db = new ApplicationDbContext();
        private UsersHelper userHelper = new UsersHelper();
        private NotificationsHelper notificationsHelper = new NotificationsHelper();
        private AccountsHelper accountHelper = new AccountsHelper();

        // GET: UploadedTransactions
        public ActionResult Index()
        {
            var uploadedTransactions = db.UploadedTransactions.Include(u => u.BankAccount).Include(u => u.IncomeExpense);
            return View(uploadedTransactions.ToList());
        }

        [HttpPost]
        public ActionResult UploadTransactions(HttpPostedFileBase uploadedFile, int BankAccountId)
        {
            var filePath = csvHelper.UploadCsv(uploadedFile);
            if (filePath == "")
            {
                return View("Error", "Shared");
            }
            var csvFile = csvHelper.ReadCsv(filePath);
            //var csvFile = csvHelper.ReadNotSaveCsv(uploadedFile);
            var nowDateTime = DateTimeOffset.Now;
            TempData["NowDateTime"] = nowDateTime;
            var expId = db.IncomeExpenses.FirstOrDefault(ie => ie.Type == "Expense").Id;
            var incId = db.IncomeExpenses.FirstOrDefault(ie => ie.Type == "Income").Id;
            UploadedTransaction upTran;
            foreach (var line in csvFile)
            {
                var alreadySaved = db.UploadedTransactions.AsNoTracking().FirstOrDefault(up => up.Created == line.Date && up.Description == line.Description && up.Amount == line.Amount);
                if (alreadySaved == null)
                {
                    upTran = new UploadedTransaction();
                    upTran.Amount = line.Amount;
                    upTran.Description = line.Description;
                    upTran.Created = line.Date;
                    upTran.BankAccountId = BankAccountId;
                    upTran.UploadDateTime = nowDateTime;
                    upTran.Saved = false;
                    if (upTran.Amount < 0)
                    {
                        upTran.IncomeExpenseId = expId;
                    }
                    else
                    {
                        upTran.IncomeExpenseId = incId;
                    }
                    db.UploadedTransactions.Add(upTran);

                    db.SaveChanges();
                }
            }
            TempData["CsvData"] = csvFile.OrderByDescending(x => x.Date).ToList();
            csvHelper.DeleteCsv(filePath);

            //User and household validation stuff
            //Populate Viewbag data

            if (!userHelper.UserHasEverything())
            {
                TempData["Error"] = "You must have a household, budget, and bank account(s) first!";
                return RedirectToAction("ErrorPage", "Home");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            var budget = new Budget();
            var ExpenseId = db.IncomeExpenses.FirstOrDefault(i => i.Type == "Expense").Id;
            if (user != null && user.HouseholdId != null)
            {
                ViewBag.hId = user.HouseholdId;
                budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(a => a.HouseholdId == user.HouseholdId).ToList(), "Id", "Name");
                if (budget != null)
                {
                    var budgetItemList = budget.BudgetItems.ToList();
                    var item = budgetItemList[0];
                    ViewBag.BudgetItemList = new SelectList(budgetItemList, "Id", "Name");
                }
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", BankAccountId);
            ViewBag.SelectedBankAccountId = BankAccountId;
            ViewBag.BankAccountName = db.BankAccounts.FirstOrDefault(ba => ba.Id == BankAccountId).Name;
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", ExpenseId);
            //return RedirectToAction("ReadCSV", new { CsvFile = filePath });
            var transList = db.UploadedTransactions.Where(t => !t.Saved).OrderByDescending(x => x.Created).ToList();
            return View(transList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveTransaction([Bind(Include = "Description,Amount,BudgetItemId,BankAccountId,Created,IncomeExpenseId")] Transaction transaction, int UploadedTransactionId)
        //public async Task<ActionResult> SaveTransaction(string Description, decimal Amount, int BudgetItemId, int BankAccountId, DateTimeOffset Created, int IncomeExpenseId, int UploadedTransactionId)
        {
            //var transaction = new Transaction();
        
            if (!userHelper.UserHasEverything())
            {
                TempData["Error"] = "You must have a household, budget, and bank account(s) first!";
                return RedirectToAction("ErrorPage", "Home");
            }

            var upTrans = db.UploadedTransactions.FirstOrDefault(up => up.Id == UploadedTransactionId);
            var user = db.Users.Find(User.Identity.GetUserId());
            transaction.EnteredById = user.Id;
            //transaction.Description = Description;
            //transaction.Created = Created;
            //transaction.Amount = Amount;
            //transaction.BudgetItemId = BudgetItemId;
            //transaction.BankAccountId = BankAccountId;
            //transaction.IncomeExpenseId = IncomeExpenseId;

            //Add to db and save changes
            upTrans.Saved = true;
            db.Entry(upTrans).State = EntityState.Modified;
            db.Transactions.Add(transaction);
            db.SaveChanges();

            //Send emails and notifications
            #region Send Balance Notificationsa and emails
            var account = db.BankAccounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);
            var recipientEmail = account.AccountOwner.Email;
            var accountBalance = accountHelper.GetAccountBalance(account.Id);
            if (accountBalance <= account.WarningBalance)
            {
                var callbackUrl = Url.Action("ViewTransactionAlert", "Transactions", new { transId = transaction.Id }, protocol: Request.Url.Scheme);
                var message = new IdentityMessage();
                message.Destination = account.AccountOwner.Email;
                message.Subject = "Account Balance Warning";
                message.Body = account.Name + " has a balance of $" + accountBalance + ". Please click <a href=\"" + callbackUrl + "\">here</a> to view";
                var emailSvc = new EmailService();
                await emailSvc.SendAsync(message);

                notificationsHelper.CreateNotification(message);
            }

            #endregion           

            /*Using RedirectToAction to clear residual SelectList data, so last selection does not appear in all of the
              Budget Item Lists.*/
            return RedirectToAction("NextTransaction");
        }

        public ActionResult NextTransaction()
        {
            var transList = db.UploadedTransactions.Where(t => !t.Saved).OrderByDescending(x => x.Created).ToList();
            return View("UploadTransactions", transList);
        }

        // GET: UploadedTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UploadedTransaction uploadedTransaction = db.UploadedTransactions.Find(id);
            if (uploadedTransaction == null)
            {
                return HttpNotFound();
            }
            return View(uploadedTransaction);
        }

        // GET: UploadedTransactions/Create
        public ActionResult Create()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type");
            return View();
        }

        // POST: UploadedTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Amount,Created,Saved,UploadDateTime,BankAccountId,IncomeExpenseId")] UploadedTransaction uploadedTransaction)
        {
            if (ModelState.IsValid)
            {
                db.UploadedTransactions.Add(uploadedTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", uploadedTransaction.BankAccountId);
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", uploadedTransaction.IncomeExpenseId);
            return View(uploadedTransaction);
        }

        // GET: UploadedTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UploadedTransaction uploadedTransaction = db.UploadedTransactions.Find(id);
            if (uploadedTransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", uploadedTransaction.BankAccountId);
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", uploadedTransaction.IncomeExpenseId);
            return View(uploadedTransaction);
        }

        // POST: UploadedTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Amount,Created,Saved,UploadDateTime,BankAccountId,IncomeExpenseId")] UploadedTransaction uploadedTransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uploadedTransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", uploadedTransaction.BankAccountId);
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", uploadedTransaction.IncomeExpenseId);
            return View(uploadedTransaction);
        }

        // GET: UploadedTransactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UploadedTransaction uploadedTransaction = db.UploadedTransactions.Find(id);
            if (uploadedTransaction == null)
            {
                return HttpNotFound();
            }
            return View(uploadedTransaction);
        }

        // POST: UploadedTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UploadedTransaction uploadedTransaction = db.UploadedTransactions.Find(id);
            db.UploadedTransactions.Remove(uploadedTransaction);
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
