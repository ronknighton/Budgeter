using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class BudgetsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ICollection<BudgetItem> ListBudgetItems(int budgetId)
        {
            return db.Budgets.FirstOrDefault(b => b.Id == budgetId).BudgetItems.ToList();
        }

        public void RemoveBudgetItem(int budgetId, int itemId)
        {
            var budget = db.Budgets.Include(b => b.BudgetItems).FirstOrDefault(b => b.Id == budgetId);
            var item = db.BudgetItems.FirstOrDefault(i => i.Id == itemId);

            item.Active = false;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void AddBudgetItem(int budgetId, int itemId)
        {
            var budget = db.Budgets.Include(b => b.BudgetItems).FirstOrDefault(b => b.Id == budgetId);
            var item = db.BudgetItems.FirstOrDefault(i => i.Id == itemId);

            item.Active = true;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int? GetBudgetId()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            var houseHold = new Household();
            var budget = new Budget();

            if (user != null)
            {
                var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
                if (household != null)
                {
                    budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id && b.Active);

                    if (budget != null)
                    {
                        return budget.Id;
                    }
                }

            }
            return null;
        }

        public decimal GetBudgetSpent()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            var total = 0.00M;
            var recTotal = 0.00M;
            DateTime today = DateTime.Now;
            if (user != null)
            {
                var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);

                if (household != null)
                {
                    var accounts = db.BankAccounts.Include(b => b.Transactions).Where(b => b.HouseholdId == household.Id).ToList();

                    foreach (var account in accounts)
                    {
                        foreach (var trans in account.Transactions.Where(t => !t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year && t.IncomeExpense.Type=="Expense").ToList())
                        {
                            total += trans.Amount;
                        }

                        foreach (var trans in account.Transactions.Where(t => t.Reconciled && t.Created.Month == today.Month && t.Created.Year == today.Year && t.IncomeExpense.Type == "Expense").ToList())
                        {
                            recTotal += trans.ReconciledAmount;
                        }
                    }
                }
            }
            return total + recTotal;
        }

        public decimal GetBudgetAmount()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            var houseHold = new Household();
            var budget = new Budget();

            if (user != null)
            {
                var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
                if (household != null)
                {
                    budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id && b.Active);

                    if (budget != null)
                    {
                        return budget.Amount;
                    }
                }

            }
            return 0.00M;
        }
    }
}