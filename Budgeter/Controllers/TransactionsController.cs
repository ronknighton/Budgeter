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
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;

namespace Budgeter.Controllers
{
   // [RequireHttps]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AccountsHelper accountHelper = new AccountsHelper();
        private NotificationsHelper notificationsHelper = new NotificationsHelper();
        private UsersHelper userHelper = new UsersHelper();
        private BudgetItemsHelper budgetItemsHelper = new BudgetItemsHelper();
        private HouseholdsHelper householdHelper = new HouseholdsHelper();
        private CategoriesHelper categoriesHelper = new CategoriesHelper();


        // GET: Transactions
        [NoDirectAccess]
        public ActionResult Index()
        {


            if (!userHelper.UserHasEverything())
            {
                TempData["Error"] = "You must have a household, budget, and bank account(s) first!";
                return RedirectToAction("ErrorPage", "Home");
            }
            DateTime today = DateTime.Today;
            DateTime past3Months = DateTime.Today.AddMonths(-1);
            var user = db.Users.Find(User.Identity.GetUserId());
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
            var houseHold = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
            var bankAccounts = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).ToList();
            //var transactions = db.Transactions.Include(t => t.BankAccount).Include(t => t.BudgetItem).Include(t => t.EnteredBy).Include(t => t.IncomeExpense).Where(t => t.BudgetItem.BudgetId == budget.Id).ToList();
            var allTransactions = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).SelectMany(t => t.Transactions.Where(tr => tr.Created >= past3Months)).ToList();

            ICollection<BudgetItem> listBudgetItems = db.BudgetItems.Where(b => b.BudgetId == budget.Id).ToList();
            ICollection<BudgetItemCategory> listCategories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id).ToList();
            ICollection<BankAccount> listBankAccounts = db.BankAccounts.Where(b => b.HouseholdId == user.HouseholdId).ToList();
            ICollection<IncomeExpense> listIncomeExpense = db.IncomeExpenses.ToList();

            //Pass in date ranges
            ICollection<SelectListItem> listDates = new List<SelectListItem>();
            listDates.Add(new SelectListItem() { Text = "Past 2 Months", Value = "2" });
            listDates.Add(new SelectListItem() { Text = "Past 3 Months", Value = "3" });
            listDates.Add(new SelectListItem() { Text = "Past 6 Months", Value = "6" });
            listDates.Add(new SelectListItem() { Text = "Past 12 Months", Value = "12" });

            ViewBag.Dates = new SelectList(listDates, "Value", "Text");

            ViewBag.IncomeExpense = new SelectList(listIncomeExpense, "Id", "Type");
            ViewBag.budgetItems = new SelectList(listBudgetItems, "Id", "Name");
            ViewBag.Categories = new SelectList(listCategories, "Id", "Name");
            ViewBag.BankAccounts = new SelectList(listBankAccounts, "Id", "Name");

            ViewBag.Create = true;
            return View(allTransactions);
        }

        [NoDirectAccess]
        public ActionResult EditIndex(int id)
        {
            DateTime today = DateTime.Today;
            DateTime past3Months = DateTime.Today.AddMonths(-1);
            var user = db.Users.Find(User.Identity.GetUserId());
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
            var houseHold = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
            var bankAccounts = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).ToList();

            //var transactions = db.Transactions.Include(t => t.BankAccount).Include(t => t.BudgetItem).Include(t => t.EnteredBy).Include(t => t.IncomeExpense).Where(t => t.BudgetItem.BudgetId == budget.Id).ToList();
            var allTransactions = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).SelectMany(t => t.Transactions.Where(tr => tr.Created >= past3Months)).ToList();
            ViewBag.Edit = true;
            ViewBag.Id = id;

            ICollection<BudgetItem> listBudgetItems = db.BudgetItems.Where(b => b.BudgetId == budget.Id).ToList();
            ICollection<BudgetItemCategory> listCategories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id).ToList();
            ICollection<BankAccount> listBankAccounts = db.BankAccounts.Where(b => b.HouseholdId == user.HouseholdId).ToList();
            ICollection<IncomeExpense> listIncomeExpense = db.IncomeExpenses.ToList();
            ICollection<SelectListItem> listDates = new List<SelectListItem>();
            listDates.Add(new SelectListItem() { Text = "Past 2 Months", Value = "2" });
            listDates.Add(new SelectListItem() { Text = "Past 3 Months", Value = "3" });
            listDates.Add(new SelectListItem() { Text = "Past 6 Months", Value = "6" });
            listDates.Add(new SelectListItem() { Text = "Past 12 Months", Value = "12" });

            ViewBag.IncomeExpense = new SelectList(listIncomeExpense, "Id", "Type");
            ViewBag.budgetItems = new SelectList(listBudgetItems, "Id", "Name");
            ViewBag.Categories = new SelectList(listCategories, "Id", "Name");
            ViewBag.BankAccounts = new SelectList(listBankAccounts, "Id", "Name");
            ViewBag.Dates = new SelectList(listDates, "Value", "Text");

            return View("Index", allTransactions);
        }

        [NoDirectAccess]
        public ActionResult SearchTransactions(int? BudgetItems, int? Categories, int? BankAccounts, int? IncomeExpense, string Dates)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
            var houseHold = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
            var bankAccounts = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).ToList();
            var transactions = new List<Transaction>();
            var items = new List<BudgetItem>();
            DateTime today = DateTime.Today;
            DateTime past24Months = DateTime.Today.AddMonths(-24);

            ICollection<BudgetItem> listBudgetItems = db.BudgetItems.Where(b => b.BudgetId == budget.Id).ToList();
            ICollection<BudgetItemCategory> listCategories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id).ToList();
            ICollection<BankAccount> listBankAccounts = db.BankAccounts.Where(b => b.HouseholdId == user.HouseholdId).ToList();
            ICollection<IncomeExpense> listIncomeExpense = db.IncomeExpenses.ToList();
            ICollection<SelectListItem> listDates = new List<SelectListItem>();
            listDates.Add(new SelectListItem() { Text = "Past 2 Months", Value = "2" });
            listDates.Add(new SelectListItem() { Text = "Past 3 Months", Value = "3" });
            listDates.Add(new SelectListItem() { Text = "Past 6 Months", Value = "6" });
            listDates.Add(new SelectListItem() { Text = "Past 12 Months", Value = "12" });


            // { DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(-9), DateTime.Today.AddMonths(-12) };



            ViewBag.IncomeExpense = new SelectList(listIncomeExpense, "Id", "Type");
            ViewBag.budgetItems = new SelectList(listBudgetItems, "Id", "Name", BudgetItems);
            ViewBag.Categories = new SelectList(listCategories, "Id", "Name", Categories);
            ViewBag.BankAccounts = new SelectList(listBankAccounts, "Id", "Name", BankAccounts);
            ViewBag.IncomeExpense = new SelectList(listIncomeExpense, "Id", "Type", IncomeExpense);
            ViewBag.Dates = new SelectList(listDates, "Value", "Text", Dates);

            if (BudgetItems != null)
            {
                transactions = db.Transactions.Where(t => t.BudgetItemId == (int)BudgetItems && t.Created >= past24Months).ToList();
            }
            else if (Categories != null)
            {
                items = db.BudgetItems.Where(b => b.CategoryId == Categories && b.BudgetId == budget.Id).ToList();
                foreach (var item in items)
                {
                    var theseTransactions = db.Transactions.Where(t => t.BudgetItemId == item.Id && t.Created >= past24Months).ToList();
                    transactions = transactions.Union(theseTransactions).ToList();
                }
            }
            else if (BankAccounts != null)
            {
                transactions = db.Transactions.Where(t => t.BankAccountId == BankAccounts && t.Created >= past24Months).ToList();
            }
            else if (IncomeExpense != null)
            {
                transactions = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).SelectMany(t => t.Transactions.Where(tr => tr.IncomeExpense.Id == IncomeExpense && tr.Created >= past24Months)).ToList();
            }
            else if (Dates != null)
            {
                int date = 0;
                var result = Int32.TryParse(Dates, out date);
                if (result)
                {
                    var startDate = today.AddMonths(-date);
                    //Transactions = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).SelectMany(t => t.Transactions.Where(tr => tr.Created >= past3Months)).ToList();
                    transactions = db.BankAccounts.Where(b => b.HouseholdId == houseHold.Id).SelectMany(t => t.Transactions.Where(tr => tr.Created >= startDate)).ToList();
                }

            }

            return View("Index", transactions);
        }
        //GET User Transactions
        [NoDirectAccess]
        public ActionResult UserIndex()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var transactions = db.Transactions.Include(t => t.BankAccount).Include(t => t.BudgetItem).Include(t => t.EnteredBy).Include(t => t.IncomeExpense).Where(t => t.EnteredById == user.Id).ToList();
            return View("Index", transactions);
        }

        // GET: Transactions/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            if (!userHelper.UserHasEverything())
            {
                TempData["Error"] = "You must have a household, budget, and bank account(s) first!";
                return RedirectToAction("ErrorPage", "Home");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.hId = user.HouseholdId;
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name");
            //ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type");
            var transaction = new Transaction();
            transaction.Created = DateTimeOffset.Now;
            return View(transaction);
        }

        // GET: Transactions/Create
        [NoDirectAccess]
        public ActionResult _PartialCreateTransaction()
        {
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
                    ViewBag.BudgetItemId = new SelectList(db.BudgetItems.Where(a => a.BudgetId == budget.Id).ToList(), "Id", "Name");
                }
            }

            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", ExpenseId);
            ViewBag.Create = true;
            var transaction = new Transaction();
            transaction.Created = DateTimeOffset.Now;
            return PartialView(transaction);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Description,Amount,BudgetItemId,BankAccountId,Created,IncomeExpenseId")] Transaction transaction)
        {
            if (!userHelper.UserHasEverything())
            {
                TempData["Error"] = "You must have a household, budget, and bank account(s) first!";
                return RedirectToAction("ErrorPage", "Home");
            }
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                transaction.EnteredById = user.Id;
                transaction.Created = DateTimeOffset.Now;
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
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name", transaction.BudgetItemId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", transaction.IncomeExpenseId);
            return View(transaction);
        }

       
        [NoDirectAccess]
        public ActionResult _PartialEditTransaction(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Transaction transaction = db.Transactions.Find(id);

            if (transaction == null)
            {
                return HttpNotFound();
            }

            var user = db.Users.Find(User.Identity.GetUserId());
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
            var budgetItemId = transaction.BudgetItemId;
            var incomeExpenseId = transaction.IncomeExpenseId;
            var accountId = transaction.BankAccountId;
            ViewBag.hId = user.HouseholdId;
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(a => a.HouseholdId == user.HouseholdId).ToList(), "Id", "Name", accountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems.Where(a => a.BudgetId == budget.Id).ToList(), "Id", "Name", budgetItemId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FullName");
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", incomeExpenseId);
            ViewBag.Edit = true;
            return PartialView(transaction);
        }

        // GET: Transactions/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Transaction transaction = db.Transactions.Find(id);

            if (transaction == null)
            {
                return HttpNotFound();
            }



            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name", transaction.BudgetItemId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", transaction.IncomeExpenseId);
            ViewBag.Edit = true;
            TempData["Edit"] = true;
            //return View(transaction);
            return RedirectToAction("EditIndex", new { id = id });
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Description,Amount,Created, Reconciled,ReconciledAmount,EnteredById,BudgetItemId,BankAccountId,IncomeExpenseId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.ReconciledAmount > 0)
                {
                    transaction.Reconciled = true;
                }
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();

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

                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "Name", transaction.BudgetItemId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            ViewBag.IncomeExpenseId = new SelectList(db.IncomeExpenses, "Id", "Type", transaction.IncomeExpenseId);
            return View(transaction);
        }

        //GET: View Transaction Notifications
        [Authorize]
        public ActionResult ViewTransactionAlert(int transId)
        {
            var transaction = db.Transactions.FirstOrDefault(t => t.Id == transId);
            var bankAccount = new BankAccount();
            if (transaction != null)
            {
                bankAccount = db.BankAccounts.FirstOrDefault(b => b.Id == transaction.BankAccountId);
                ViewBag.BankAccount = bankAccount;
                ViewBag.Transaction = transaction;
            }
            return View();
        }

        [NoDirectAccess]
        public ActionResult TransactionNotification(int id)
        {
            var transaction = db.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                TempData["Transaction"] = transaction;
                ViewBag.Transaction = transaction;
            }

            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.HouseholdId != null)
            {
                var members = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == user.HouseholdId).Members.ToList();
                ViewBag.RecipientId = new SelectList(members, "Id", "FullName");
            }
            else
            {
                ViewBag.NoHousehold = "You must belong to a household to send a message";
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransactionNotification([Bind(Include = "Message,RecipientId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                var sender = db.Users.Find(User.Identity.GetUserId());
                var recipient = db.Users.Find(notification.RecipientId);
                var transaction = (Transaction)TempData["Transaction"];
                var callbackUrl = Url.Action("ViewTransactionAlert", "Transactions", new { transId = transaction.Id }, protocol: Request.Url.Scheme);
                var message = new IdentityMessage();
                message.Destination = recipient.UserName;
                message.Subject = "Account Balance Warning";
                message.Body = "Message: " + notification.Message + " - Transaction Details: " + transaction.Description + " in the amount of $" + transaction.Amount + " created on " + transaction.Created + "- Please click <a href=\"" + callbackUrl + "\">here</a> to view";


                notificationsHelper.CreateNotification(message);
                return RedirectToAction("Index");
            }

            ViewBag.RecipientId = new SelectList(db.Users, "Id", "FirstName", notification.RecipientId);
            return View(notification);
        }

        // GET: Transactions/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Start chart stuff
        public ActionResult ShowCharts()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id);
            var categories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id);
            ViewBag.Categories = categories;

            return View();
        }
        public ActionResult _PartialBudgetItemsChart()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id);
            var categories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id);
            ViewBag.Categories = categories;
            return PartialView();
        }

        public ActionResult _PartialBudgetAnnualChart()
        {
            return PartialView();
        }

        [NoDirectAccess]
        public ActionResult EnterSearchDates()
        {
            var householdId = householdHelper.GetHouseholdId(); 
            if (householdId != null)
            {
                var household = db.Households.FirstOrDefault(h => h.Id == householdId);
                var householdlist = household.BankAccounts.ToList();
                ViewBag.BankAccounts = new SelectList(householdlist, "Id", "Name");
                return View();
            }


            ViewBag.BankAccounts = new SelectList(new List<BankAccount>(), "Id", "Name");
            return View();
        }


        [NoDirectAccess]
        public ActionResult GenerateReport([Bind(Include = "BeginDate, EndDate" )] DateViewModel dateTime, int bankAccountId)
        {
            ViewBag.CategoryList = categoriesHelper.GetCategoriesList().Where(c => c.BudgetItems.Any(item => item.Transactions.Count > 0));
            ViewBag.BudgetItemList = budgetItemsHelper.GetBudgetItemsList();
            
            var transactions = db.Transactions.Where(t => t.BankAccountId == bankAccountId && t.Created >= dateTime.BeginDate && t.Created <= dateTime.EndDate).ToList();

            return View(transactions);
        }

        //GET Charts
        [HttpGet]
        //Bar chart data
        //[NoDirectAccess]
        public ActionResult Charts(int? incM, int? incY, int? year, int? month)
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            var house = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
            var bankAccountIds = db.BankAccounts.Where(b => b.HouseholdId == house.Id).Select(bi => bi.Id).ToList();
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == house.Id);
            var budgetItems = db.BudgetItems.Where(b => b.BudgetId == budget.Id);
            var transactions = db.BankAccounts.Where(b => b.HouseholdId == house.Id).SelectMany(t => t.Transactions.Where(tr => tr.IncomeExpense.Type == "Expense"));

            var today = DateTimeOffset.Now;
            DateTimeOffset tod;
            if (incM != null)
            {
                tod = new DateTime((int)year, (int)month + (int)incM, today.Day);
            }
            else if (incY != null)
            {
                tod = new DateTime((int)year + (int)incY, (int)month, today.Day);
            }
            else
            {
                tod = today;
            }
            decimal totalExpense = 0;
            decimal totalBudget = 0;
            var totalAcc = accountHelper.AllAccountsBalance(bankAccountIds);
            var barlist = new List<BarChart>();
            BarChart barItem;
            var itemCount = budgetItems.ToList().Count;

            foreach (var item in budgetItems)
            {
                barItem = new BarChart();
                if (itemCount > 8)
                {
                    barItem.Name = item.Name.Length <= 9 ? item.Name : item.Name.Substring(0, 9);
                }
                else
                {
                    barItem.Name = item.Name;
                }
                barItem.Budgeted = item.Amount;
                barItem.Actual = budgetItemsHelper.GetMonthYearSpentAmount(item.Id, tod.Month, tod.Year);
                barlist.Add(barItem);
            }

            var bar = barlist.ToArray();

            //var totalAcc = house.Accounts.Select(a => a.Balance).DefaultIfEmpty().Sum();
            //Need to create an array of Budget Item stuff like below 
            //var bar = (from bi in budgetItems
            //           let aSum = (from t in transactions
            //                       where t.Created.Year == tod.Year && t.Created.Month == tod.Month
            //&& t.BudgetItemId == bi.Id
            //                       select t.Amount).DefaultIfEmpty().Sum()
            //           //let aSum = budgetItemsHelper.GetSpentAmount(bi.Id)

            //           let bSum = bi.Amount

            //           select new
            //           {                           
            //               Name = bi.Name,
            //               Actual = aSum,
            //               Budgeted = bSum
            //           }).ToArray();
            //var barList = 

            var donut = (from bi in budgetItems
                         let aSum = (from t in transactions
                                     where t.Created.Year == tod.Year && t.Created.Month == tod.Month
                                     && t.BudgetItemId == bi.Id
                                     select t.Amount).DefaultIfEmpty().Sum()
                         select new
                         {
                             label = bi.Name,
                             value = aSum
                         }).ToArray();

            var monthAndYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(tod.Month) + " " + tod.Year.ToString();
            var currentMonth = tod.Month;
            var currentYear = tod.Year;
            var result = new
            {
                totalAcc = totalAcc,
                totalBudget = totalBudget,
                totalExpense = totalExpense,
                bar = bar,
                donut = donut,
                monthYear = monthAndYear,
                currentMonth = currentMonth,
                currentYear = currentYear
            };
            //return Content(JsonConvert.SerializeObject(result), "application/json");
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        //Annual chart data
        public ActionResult GetMonthly(int? yearNum, int? inc)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.Households.Find(user.HouseholdId);
            var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id);
            int day = DateTime.Now.Day;
            int year = 0;
            List<DateTime> monthsToDate = new List<DateTime>();
            DateTime date;
            if (yearNum != null && inc != null)
            {
                for (int x = 1; x <= 12; x++)
                {
                    //new DateTime((int)year, (int)month + (int)incM, today.Day);
                    date = new DateTime((int)yearNum + (int)inc, x, day);
                    monthsToDate.Add(date);
                    year = (int)yearNum + (int)inc;
                }
            }
            else
            {
                monthsToDate = Enumerable.Range(1, DateTime.Today.Month).Select(m => new DateTime(DateTime.Today.Year, m, 1)).ToList();
                year = DateTime.Now.Year;
            }


            var sums = from month in monthsToDate
                       select new
                       {
                           month = month.ToString("MMM"),
                           income = (from account in household.BankAccounts
                                     from transaction in account.Transactions
                                     where transaction.IncomeExpense.Type == "Income" &&
                                     transaction.Created.Month == month.Month && transaction.Created.Year == year
                                     select transaction.Amount).DefaultIfEmpty().Sum(),
                           //income = household.Accounts.SelectMany(t => t.Transactions).Where(c =>
                           //    c.Category.CategoryType.Name == "Income" && c.TransDate.Month ==
                           //month.Month).Select(t => t.Amount).DefaultIfEmpty().Sum(),
                           expense = (from account in household.BankAccounts
                                      from transaction in account.Transactions
                                      where transaction.IncomeExpense.Type == "Expense"
                                      && transaction.Created.Month == month.Month && transaction.Created.Year == year
                                      select transaction.Amount).DefaultIfEmpty().Sum(),
                           //expenses = household.Accounts.SelectMany(a => a.Transactions).Where(t =>
                           //t.Category.CategoryType.Name == "Expense" && t.TransDate.Month == month.Month).Select(t => t.Amount).DefaultIfEmpty().Sum(),
                           budget = budget.Amount,
                           selectedYear = year
                       };


            return Content(JsonConvert.SerializeObject(sums), "application/json");
        }



        public ActionResult GetCategoryData(int? id, int? month, int? year)
        {
            //month = DateTime.Now.Month;
            //year = DateTime.Now.Year;
            var result = new PieChart();

            if (id != null)
            {
                var category = db.BudgetItemCategories.Include(c => c.BudgetItems).FirstOrDefault(c => c.Id == id);
                var budgetItems = db.BudgetItems.Where(b => b.CategoryId == id);
                var transactions = db.BudgetItems.SelectMany(t => t.Transactions.Where(b => b.BudgetItem.CategoryId == id && b.Created.Month == month && b.Created.Year == year));

                var count = 0;
                var color = 580909;
                var colors = new List<string>();
                var labels = new List<string>();
                var data = new List<decimal>();
                var amount = 0.00M;
                var recAmount = 0.00M;
                foreach (var bi in category.BudgetItems)
                {
                    labels.Add(bi.Name);
                    amount = bi.Transactions.Where(t => t.Created.Year == year && t.Created.Month == month && t.IncomeExpense.Type == "Expense" && !t.Reconciled).Select(t => t.Amount).Sum();
                    recAmount = bi.Transactions.Where(t => t.Created.Year == year && t.Created.Month == month && t.IncomeExpense.Type == "Expense" && t.Reconciled).Select(t => t.ReconciledAmount).Sum();
                    data.Add(amount + recAmount);
                    color += (int)year;
                    colors.Add("#" + color);
                    count++;
                }
                result.CatName = category.Name;
                result.Labels = labels.ToArray();
                result.Data = data.ToArray();
                result.Colors = colors.ToArray();
            }

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        public class PieChart
        {
            public string CatName { get; set; }
            public string[] Labels { get; set; }
            public decimal[] Data { get; set; }
            public string[] Colors { get; set; }

        }

        public class BarChart
        {
            public string Name { get; set; }
            public decimal Actual { get; set; }
            public decimal Budgeted { get; set; }
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
