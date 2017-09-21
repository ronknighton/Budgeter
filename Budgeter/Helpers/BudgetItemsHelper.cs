using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class BudgetItemsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<int> GetBudgetItems()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            var houseHold = new Household();
            var budget = new Budget();
            var budgetItemsId = new List<int>();
            if (user != null)
            {
                var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
                if (household != null)
                {
                    budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id && b.Active);

                    if (budget != null)
                    {
                        budgetItemsId = db.BudgetItems.Where(b => b.BudgetId == budget.Id && b.Active).Select(b => b.Id).ToList();
                    }
                }
                return budgetItemsId;
            }
            return new List<int>();
        }

        public int GetItemPercentage(int id)
        {
            DateTime today = DateTime.Today;
            var budgetAmount = db.BudgetItems.FirstOrDefault(b => b.Id == id).Amount;
            var itemTotalIncome = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Income" && !t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.Amount).ToList().Sum();
            var itemTotalExpense = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Expense" && !t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.Amount).ToList().Sum();
            var itemTotalReconciledIncome = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Income" && t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.ReconciledAmount).ToList().Sum();
            var itemTotalReconciledExpense = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Expense" && t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.ReconciledAmount).ToList().Sum();

            var total = itemTotalIncome + itemTotalReconciledIncome + itemTotalExpense + itemTotalReconciledExpense;
            if (budgetAmount > 0)
            {
                var percent = (int)Math.Round((total / budgetAmount) * 100);
                return percent;
            }
            else
            {
                return 0;
            }

        }

        public decimal GetSpentAmount(int id)
        {
            DateTime today = DateTime.Today;
            var budgetAmount = db.BudgetItems.FirstOrDefault(b => b.Id == id).Amount;
            var itemTotalIncome = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Income" && !t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.Amount).ToList().Sum();
            var itemTotalExpense = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Expense" && !t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.Amount).ToList().Sum();
            var itemTotalReconciledIncome = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Income" && t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.ReconciledAmount).ToList().Sum();
            var itemTotalReconciledExpense = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Expense" && t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year).Select(t => t.ReconciledAmount).ToList().Sum();

            var total = itemTotalIncome + itemTotalReconciledIncome + itemTotalExpense + itemTotalReconciledExpense;
            return total;
        }

        public decimal GetMonthYearSpentAmount(int id, int month, int year)
        {
            DateTime today = DateTime.Today;
            var budgetAmount = db.BudgetItems.FirstOrDefault(b => b.Id == id).Amount;
            var itemTotalIncome = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Income" && !t.Reconciled && t.Created.Month == month && t.Created.Year == year).Select(t => t.Amount).ToList().Sum();
            var itemTotalExpense = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Expense" && !t.Reconciled && t.Created.Month == month && t.Created.Year == year).Select(t => t.Amount).ToList().Sum();
            var itemTotalReconciledIncome = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Income" && t.Reconciled && t.Created.Month == month && t.Created.Year == year).Select(t => t.ReconciledAmount).ToList().Sum();
            var itemTotalReconciledExpense = db.Transactions.Where(t => t.BudgetItemId == id && t.IncomeExpense.Type == "Expense" && t.Reconciled && t.Created.Month == month && t.Created.Year == year).Select(t => t.ReconciledAmount).ToList().Sum();

            var total = itemTotalIncome + itemTotalReconciledIncome + itemTotalExpense + itemTotalReconciledExpense;
            return total;
        }
        public decimal GetBudgetAmount(int id)
        {
            return db.BudgetItems.FirstOrDefault(b => b.Id == id).Amount;
        }

        public string getBudgetItemName(int id)
        {
            var name = db.BudgetItems.FirstOrDefault(b => b.Id == id).Name;
            return name;
        }

        public string GetBarColor(int percent)
        {
            if (percent < 50)
            {
                return "progress-bar-info";
            }
            if (percent >= 50 && percent <= 75)
            {
                return "progress-bar-success";
            }
            if (percent > 75 & percent <= 100)
            {
                return "progress-bar-warning";
            }
            else return "progress-bar-danger";
        }

        public string GetMonth()
        {
            var month = DateTime.Today.Month;
            return DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
        }

    }
}